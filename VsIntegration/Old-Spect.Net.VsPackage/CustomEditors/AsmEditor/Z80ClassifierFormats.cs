using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// Defines an editor format for a Z80 assembly label
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Label")]
    [Name("Z80Label")]
    [UserVisible(true)] 
    [Order(Before = Priority.Default)]
    internal sealed class Z80LabelClassifierFormat : ClassificationFormatDefinition
    {
        public Z80LabelClassifierFormat()
        {
            DisplayName = "Z80 Asm - Label";
            ForegroundColor = Colors.DarkOrange;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly pragma
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Pragma")]
    [Name("Z80Pragma")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80PragmaClassifierFormat : ClassificationFormatDefinition
    {
        public Z80PragmaClassifierFormat()
        {
            DisplayName = "Z80 Asm - Pragma";
            ForegroundColor = Colors.CadetBlue;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly directive
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Directive")]
    [Name("Z80Directive")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80DirectiveClassifierFormat : ClassificationFormatDefinition
    {
        public Z80DirectiveClassifierFormat()
        {
            DisplayName = "Z80 Asm - Directive";
            ForegroundColor = Colors.LightGray;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly include directive
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80IncludeDirective")]
    [Name("Z80IncludeDirective")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80IncludeDirectiveClassifierFormat : ClassificationFormatDefinition
    {
        public Z80IncludeDirectiveClassifierFormat()
        {
            DisplayName = "Z80 Asm - Include directive";
            ForegroundColor = Colors.Silver;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly instruction
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Instruction")]
    [Name("Z80Instruction")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80InstructionClassifierFormat : ClassificationFormatDefinition
    {
        public Z80InstructionClassifierFormat()
        {
            DisplayName = "Z80 Asm - Instruction";
            ForegroundColor = Colors.NavajoWhite;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly comment
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Comment")]
    [Name("Z80Comment")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80CommentClassifierFormat : ClassificationFormatDefinition
    {
        public Z80CommentClassifierFormat()
        {
            DisplayName = "Z80 Asm - Comment";
            ForegroundColor = Colors.LimeGreen;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly number
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Number")]
    [Name("Z80Number")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80NumberClassifierFormat : ClassificationFormatDefinition
    {
        public Z80NumberClassifierFormat()
        {
            DisplayName = "Z80 Asm - Number";
            ForegroundColor = Colors.Cyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly string
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80String")]
    [Name("Z80String")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80StringClassifierFormat : ClassificationFormatDefinition
    {
        public Z80StringClassifierFormat()
        {
            DisplayName = "Z80 Asm - String";
            ForegroundColor = Colors.Cyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly identifier
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Identifier")]
    [Name("Z80Identifier")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80IdentifierClassifierFormat : ClassificationFormatDefinition
    {
        public Z80IdentifierClassifierFormat()
        {
            DisplayName = "Z80 Asm - Identifier";
            ForegroundColor = Colors.DarkCyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly function
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Function")]
    [Name("Z80Function")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80FunctionClassifierFormat : ClassificationFormatDefinition
    {
        public Z80FunctionClassifierFormat()
        {
            DisplayName = "Z80 Asm - Function";
            ForegroundColor = Colors.DarkCyan;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 breakpoint line
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Breakpoint")]
    [Name("Z80Breakpoint")]
    [UserVisible(true)]
    [Order(Before = "Z80CurrentBreakpoint")]
    internal sealed class Z80BreakpointClassifierFormat : ClassificationFormatDefinition
    {
        public Z80BreakpointClassifierFormat()
        {
            DisplayName = "Z80 Asm - Breakpoint";
            BackgroundColor = Colors.Red;
            BackgroundOpacity = 0.4;
        }
    }

    /// <summary>
    /// Defines an editor format for the current Z80 breakpoint line
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80CurrentBreakpoint")]
    [Name("Z80CurrentBreakpoint")]
    [UserVisible(true)]
    [Order(After = "Z80Breakpoint")]
    internal sealed class Z80CurrentBreakpointClassifierFormat : ClassificationFormatDefinition
    {
        public Z80CurrentBreakpointClassifierFormat()
        {
            DisplayName = "Z80 Asm - Current breakpoint";
            BackgroundColor = Colors.Orange;
            
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly macro parameter
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80MacroParam")]
    [Name("Z80MacroParam")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80MacroParamFormat : ClassificationFormatDefinition
    {
        public Z80MacroParamFormat()
        {
            DisplayName = "Z80 Asm - Macro Parameter";
            ForegroundColor = Colors.DarkOrchid;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly statement
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Statement")]
    [Name("Z80Statement")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80StatementFormat : ClassificationFormatDefinition
    {
        public Z80StatementFormat()
        {
            DisplayName = "Z80 Asm - Statement";
            ForegroundColor = Colors.OliveDrab;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly module keyword
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Module")]
    [Name("Z80Module")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80ModuleFormat : ClassificationFormatDefinition
    {
        public Z80ModuleFormat()
        {
            DisplayName = "Z80 Asm - Module";
            ForegroundColor = Colors.Yellow;
            BackgroundColor = Color.FromArgb(255, 80, 80, 80);
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly macro invocation
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80MacroInvocation")]
    [Name("Z80MacroInvocation")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80MacroInvocationFormat : ClassificationFormatDefinition
    {
        public Z80MacroInvocationFormat()
        {
            DisplayName = "Z80 Asm - Invoke macro";
            ForegroundColor = Colors.OliveDrab;
            IsItalic = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 operand
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80Operand")]
    [Name("Z80Operand")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80OperandClassifierFormat : ClassificationFormatDefinition
    {
        public Z80OperandClassifierFormat()
        {
            DisplayName = "Z80 Asm - Operand";
            ForegroundColor = Colors.NavajoWhite;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 operand
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Z80SemiVar")]
    [Name("Z80SemiVar")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80SemiVarClassifierFormat : ClassificationFormatDefinition
    {
        public Z80SemiVarClassifierFormat()
        {
            DisplayName = "Z80 Asm - Semi-variables";
            ForegroundColor = Colors.LightCoral;
            IsItalic = true;
        }
    }
}
