using System;
using System.Collections.Generic;
using System.Reflection;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace CommandLineParser.Arguments
{
    /// <summary>
    /// Switch argument can be used to represent options with true/false logic. It is initialized with default value and
    /// when the argument appears on the command line, the value is flipped. 
    /// </summary>
    /// <include file='Doc\CommandLineParser.xml' path='CommandLineParser/Arguments/SwitchArgument/*'/>
    public class SwitchArgument: Argument
    {
        #region property backing fieldds
        
        private bool value;

        private bool defaultValue;

        #endregion

        #region constructors

        /// <summary>
        /// Creates new switch argument with a <see cref="Argument.ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="defaultValue">default value of the argument</param>
        public SwitchArgument(char shortName, bool defaultValue) : base(shortName)
        {
            this.defaultValue = defaultValue;
            this.value = defaultValue;
        }

        /// <summary>
        /// Creates new switch argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="defaultValue">default value of the argument</param>
        public SwitchArgument(char shortName, string longName, bool defaultValue) : base(shortName, longName)
        {
            this.defaultValue = defaultValue;
            this.value = defaultValue;
        }

        /// <summary>
        /// Creates new switch argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">description of the argument</param>
        /// <param name="defaultValue">default value of the argument</param>
        public SwitchArgument(char shortName, string longName, string description, bool defaultValue) : base(shortName, longName, description)
        {
            this.defaultValue = defaultValue;
            this.value = defaultValue;
        }

        #endregion

        /// <summary>
        /// Value of the switch argument
        /// </summary>
        public bool Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Default value of the switch argument. Restored each time <see cref="Init"/> is called.
        /// </summary>
        public bool DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }


        /// <summary>
        /// Parse argument. This method reads the argument from the input field and moves the 
        /// index to the next argument.
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <param name="i">index to the args array, where this argument occured. </param>
        internal override void Parse(IList<string> args, ref int i)
        {
            base.Parse(args, ref i);
            value = !value;
            Parsed = true; 
            i++;
        }

        /// <summary>
        /// If <see cref="Argument.Bind"/> is specified, the bound field of the bound object should is updated
        /// according to the value of the argument. Should be called after Parse is called.  
        /// </summary>
        public override void UpdateBoundObject()
        {
            if (Bind.HasValue)
            {
                try
                {
                    Bind.Value.Object.GetType().InvokeMember(Bind.Value.Field,
                                                             BindingFlags.SetField | BindingFlags.SetProperty,
                                                             null, Bind.Value.Object, new object[] {value});
                }
                catch (Exception e)
                {
                    throw new CommandLineException(
                        string.Format(Messages.EXC_BINDING, Name,
                                      Bind.Value.Field, Bind.Value.Object), e);
                }
            }
        }

        /// <summary>
        /// Prints information about the argument value to the console.
        /// </summary>
        internal override void PrintValueInfo()
        {
            Console.WriteLine(Messages.EXC_ARG_SWITCH_PRINT, Name, value == true ? "1" : "0");
        }

        /// <summary>
        /// Initializes the argument and restores the <see cref="DefaultValue"/>.
        /// </summary>
        public override void Init()
        {
            base.Init();
            this.value = defaultValue;
        }
    }
   
    /// <summary>
    /// <para>
    /// Attribute for declaring a class' field a <see cref="SwitchArgument"/> and 
    /// thus binding a field's value to a certain command line switch argument.
    /// </para>
    /// <para>
    /// Instead of creating an argument explicitly, you can assign a class' field an argument
    /// attribute and let the CommandLineParse take care of binding the attribute to the field.
    /// </para>
    /// </summary>
    /// <remarks>Appliable to fields and properties (public).</remarks>
    /// <remarks>Use <see cref="CommandLineParser.ExtractArgumentAttributes"/> for each object 
    /// you where you have delcared argument attributes.</remarks>
    /// <example>
    /// <code source="Examples\AttributeExample.cs" lang="cs" title="Example of declaring argument attributes" />
    /// </example>
    public class SwitchArgumentAttribute: ArgumentAttribute
    {
        /// <summary>
        /// Creates new instance of SwitchArgumentAtribute.
        /// </summary>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="defaultValue"><see cref="SwitchArgument.DefaultValue">default value</see> of the underlying argument</param>
        public SwitchArgumentAttribute(char shortName, bool defaultValue)
            : base(typeof(SwitchArgument), shortName, defaultValue)
        {
        }

        /// <summary>
        /// Creates new instance of SwitchArgumentAtribute.
        /// </summary>
        /// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
        /// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
        /// <param name="defaultValue"><see cref="SwitchArgument.DefaultValue">default value</see> of the underlying argument</param>
        public SwitchArgumentAttribute(char shortName, string longName, bool defaultValue)
            : base(typeof(SwitchArgument), shortName, longName, defaultValue)
        {
        }
    }
}