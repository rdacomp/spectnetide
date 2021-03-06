using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents a function invocation
    /// </summary>
    public class FunctionInvocationNode : ExpressionNode
    {
        private static Random s_Random;

        /// <summary>
        /// Initialize the static members of the class
        /// </summary>
        static FunctionInvocationNode()
        {
            s_Random = new Random((int)DateTime.Now.Ticks);
        }

        /// <summary>
        /// Sets the seed value of the rand() function
        /// </summary>
        /// <param name="value"></param>
        public static void SetRandomSeed(ExpressionValue value)
        {
            if (value == null || !value.IsValid)
            {
                s_Random = new Random((int)DateTime.Now.Ticks);
            }
            else
            {
                s_Random = new Random(value.Value);
            }
        }

        /// <summary>
        /// The name of the function
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// The list of argument expressions
        /// </summary>
        public List<ExpressionNode> ArgumentExpressions { get; }

        /// <summary>
        /// Initializes the function invocation
        /// </summary>
        public FunctionInvocationNode(Z80AsmParser.FunctionInvocationContext context, Z80AsmVisitor visitor)
            : base(context)
        {
            FunctionName = context.IDENTIFIER()?.GetText()?.ToLower();
            if (FunctionName != null)
            {
                visitor.AddFunction(context);
            }
            ArgumentExpressions = context.expr().Select(expr => (ExpressionNode)visitor.VisitExpr(expr)).ToList();
        }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IEvaluationContext evalContext) => 
            ArgumentExpressions.TrueForAll(expr => expr.ReadyToEvaluate(evalContext));

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            // --- Evaluate all arguments from left to right
            var argValues = new List<ExpressionValue>();
            var errorMessage = new StringBuilder(1024);
            var index = 0;
            var errCount = 0;
            foreach (var expr in ArgumentExpressions)
            {
                index++;
                var argValue = expr.Evaluate(evalContext);
                if (argValue == null)
                {
                    errCount++;
                    errorMessage.AppendLine($"Arg #{index}: {expr.EvaluationError}");
                }
                else
                {
                    argValues.Add(argValue);
                }
            }

            // --- Check for evaluation errors
            if (errCount > 0)
            {
                EvaluationError = $"Function argument evaluation failed:\n {errorMessage}";
                return ExpressionValue.Error;
            }

            // --- Function must be defined
            if (!s_Evaluators.TryGetValue(FunctionName, out var evaluator))
            {
                EvaluationError = $"Unknown function '{FunctionName}'";
                return ExpressionValue.Error;
            }

            // --- Find the apropriate signature
            FunctionEvaluator evaluatorFound = null;
            foreach (var evalOption in evaluator)
            {
                if (evalOption.ArgTypes.Length != ArgumentExpressions.Count) continue;

                // --- A viable option found
                var match = true;
                for (var i = 0; i < evalOption.ArgTypes.Length; i++)
                {
                    var type = argValues[i].Type;
                    switch (evalOption.ArgTypes[i])
                    {
                        case ExpressionValueType.Bool:
                            match = type == ExpressionValueType.Bool;
                            break;
                        case ExpressionValueType.Integer:
                            match = type == ExpressionValueType.Bool
                                    || type == ExpressionValueType.Integer;
                            break;
                        case ExpressionValueType.Real:
                            match = type == ExpressionValueType.Bool
                                    || type == ExpressionValueType.Integer
                                    || type == ExpressionValueType.Real;
                            break;
                        case ExpressionValueType.String:
                            match = type == ExpressionValueType.String;
                            break;
                        default:
                            return ExpressionValue.Error;
                    }

                    // --- Abort search if the current argumernt type does not match
                    if (!match) break;
                }

                if (match)
                {
                    // --- We have found a matching signature
                    evaluatorFound = evalOption;
                    break;
                }
            }

            // --- Check whether we found an option
            if (evaluatorFound == null)
            {
                EvaluationError = $"The arguments of '{FunctionName}' do not match any acceptable signatures";
                return ExpressionValue.Error;

            }

            // --- Now, it is time to evaluate the function
            try
            {
                var functionValue = evaluatorFound.EvaluateFunc(argValues);
                return functionValue;
            }
            catch (Exception e)
            {
                EvaluationError = $"Exception while evaluating '{FunctionName}': {e.Message}";
                return ExpressionValue.Error;
            }
        }

        /// <summary>
        /// This class describes a function evaluator
        /// </summary>
        internal class FunctionEvaluator
        {
            /// <summary>
            /// Argument Types
            /// </summary>
            public ExpressionValueType[] ArgTypes { get; }

            /// <summary>
            /// Function evaluation
            /// </summary>
            public Func<IList<ExpressionValue>, ExpressionValue> EvaluateFunc { get; }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public FunctionEvaluator(Func<IList<ExpressionValue>, ExpressionValue> evaluateFunc, params ExpressionValueType[] argTypes)
            {
                ArgTypes = argTypes;
                EvaluateFunc = evaluateFunc;
            }
        }

        /// <summary>
        /// Declares the function evaluator methods
        /// </summary>
        private static readonly Dictionary<string, IList<FunctionEvaluator>> s_Evaluators = 
            new Dictionary<string, IList<FunctionEvaluator>>
        {
            { "abs", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Abs(args[0].AsLong())), ExpressionValueType.Integer),
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Abs(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "acos", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Acos(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "asin", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Asin(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "atan", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Atan(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "atan2", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            Math.Atan2(args[0].AsReal(), args[1].AsReal())), 
                        ExpressionValueType.Real, ExpressionValueType.Real)
                }
            },
            { "ceiling", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue((long)Math.Ceiling(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "cos", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Cos(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "cosh", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Cosh(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "exp", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Exp(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "floor", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue((long)Math.Floor(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "log", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Log(args[0].AsReal())), ExpressionValueType.Real),
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            Math.Log(args[0].AsReal(), args[1].AsReal())),
                        ExpressionValueType.Real, ExpressionValueType.Real)
                }
            },
            { "log10", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Log10(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "max", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            Math.Max(args[0].AsLong(), args[1].AsLong())),
                        ExpressionValueType.Integer, ExpressionValueType.Integer),
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            Math.Max(args[0].AsReal(), args[1].AsReal())),
                        ExpressionValueType.Real, ExpressionValueType.Real)
                }
            },
            { "min", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            Math.Min(args[0].AsLong(), args[1].AsLong())),
                        ExpressionValueType.Integer, ExpressionValueType.Integer),
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            Math.Min(args[0].AsReal(), args[1].AsReal())),
                        ExpressionValueType.Real, ExpressionValueType.Real)
                }
            },
            { "pow", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            Math.Pow(args[0].AsReal(), args[1].AsReal())),
                        ExpressionValueType.Real, ExpressionValueType.Real)
                }
            },
            { "round", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Round(args[0].AsReal())), ExpressionValueType.Real),
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            Math.Round(args[0].AsReal(), (int)args[1].AsLong())),
                        ExpressionValueType.Real, ExpressionValueType.Integer)
                }
            },
            { "sign", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Sign(args[0].AsLong())), ExpressionValueType.Integer),
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Sign(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "sin", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Sin(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "sinh", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Sinh(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "sqrt", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Sqrt(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "tan", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Tan(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "tanh", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.Tanh(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "truncate", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue((long)Math.Truncate(args[0].AsReal())), ExpressionValueType.Real)
                }
            },
            { "pi", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.PI))
                }
            },
            { "nat", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(Math.E))
                }
            },
            { "low", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue((byte)args[0].AsLong()), ExpressionValueType.Integer)
                }
            },
            { "high", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue((byte)(args[0].AsLong() >> 8)), ExpressionValueType.Integer)
                }
            },
            { "word", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue((ushort)args[0].AsLong()), ExpressionValueType.Integer)
                }
            },
            { "rnd", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue((uint)s_Random.Next(int.MinValue, int.MaxValue))),
                    new FunctionEvaluator(
                        args => new ExpressionValue(
                            (uint)s_Random.Next((int)args[0].AsLong(), (int)args[1].AsLong())), 
                        ExpressionValueType.Integer, ExpressionValueType.Integer)
                }
            },
            { "length", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString().Length), ExpressionValueType.String)
                }
            },
            { "len", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString().Length), ExpressionValueType.String)
                }
            },
            { "left", new []
                {
                    new FunctionEvaluator(
                        args =>
                        {
                            var str = args[0].AsString();
                            var len = Math.Min(str.Length, (int) args[1].AsLong());
                            return new ExpressionValue(str.Substring(0, len));
                        }, 
                        ExpressionValueType.String,
                        ExpressionValueType.Integer)
                }
            },
            { "right", new []
                {
                    new FunctionEvaluator(
                        args =>
                        {
                            var str = args[0].AsString();
                            var len = Math.Min(str.Length, (int) args[1].AsLong());
                            return new ExpressionValue(str.Substring(str.Length-len, len));
                        },
                        ExpressionValueType.String,
                        ExpressionValueType.Integer)
                }
            },
            { "substr", new []
                {
                    new FunctionEvaluator(
                        args =>
                        {
                            var str = args[0].AsString();
                            var start = Math.Min(str.Length, (int) args[1].AsLong());
                            var len = Math.Min(str.Length - start, (int) args[2].AsLong());
                            return new ExpressionValue(str.Substring(start, len));
                        },
                        ExpressionValueType.String,
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer)
                }
            },
            { "fill", new []
                {
                    new FunctionEvaluator(
                        args =>
                        {
                            var str = args[0].AsString();
                            var count = (int) args[1].AsLong();
                            var resultLen = str.Length * count;
                            if (resultLen > 0x4000)
                            {
                                throw new InvalidOperationException("The result of the fill() function would be longer than #4000 bytes.");
                            }
                            var result = new StringBuilder(resultLen + 10);
                            for (var i = 0; i < count; i++)
                            {
                                result.Append(str);
                            }

                            return new ExpressionValue(result.ToString());
                        },
                        ExpressionValueType.String,
                        ExpressionValueType.Integer)
                }
            },
            { "int", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsLong()), ExpressionValueType.Real)
                }
            },
            { "frac", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsReal() - args[0].AsLong()), ExpressionValueType.Real)
                }
            },
            { "lowercase", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString().ToLower()), ExpressionValueType.String)
                }
            },
            { "lcase", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString().ToLower()), ExpressionValueType.String)
                }
            },
            { "uppercase", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString().ToUpper()), ExpressionValueType.String)
                }
            },
            { "ucase", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString().ToUpper()), ExpressionValueType.String)
                }
            },
            { "str", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString()), ExpressionValueType.Bool),
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString()), ExpressionValueType.Integer),
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString()), ExpressionValueType.Real),
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsString()), ExpressionValueType.String)
                }
            },
            { "scraddr", new []
                {
                    new FunctionEvaluator(
                        args =>
                        {
                            var line = args[0].AsLong();
                            if (line < 0 || line > 191)
                            {
                                throw new InvalidOperationException(
                                    $"The 'line' argument of scraddr must be between 0 and 191. It cannot be {line}.");
                            }

                            var col = args[1].AsLong();
                            if (col < 0 || col > 255)
                            {
                                throw new InvalidOperationException(
                                    $"The 'col' argument of scraddr must be between 0 and 255. It cannot be {col}.");
                            }
                            var da = 0x4000 | (col >> 3) | (line << 5);
                            var addr = (ushort)((da & 0xF81F)
                                            | ((da & 0x0700) >> 3)
                                            | ((da & 0x00E0) << 3));
                            return new ExpressionValue(addr);
                        },
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer)
                }
            },
            { "attraddr", new []
                {
                    new FunctionEvaluator(
                        args =>
                        {
                            var line = args[0].AsLong();
                            if (line < 0 || line > 191)
                            {
                                throw new InvalidOperationException(
                                    $"The 'line' argument of attraddr must be between 0 and 191. It cannot be {line}.");
                            }

                            var col = args[1].AsLong();
                            if (col < 0 || col > 255)
                            {
                                throw new InvalidOperationException(
                                    $"The 'col' argument of attraddr must be between 0 and 255. It cannot be {col}.");
                            }
                            return new ExpressionValue(0x5800 + (line >> 3) * 32 + (col >> 3) );
                        },
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer)
                }
            },
            { "ink", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsLong() & 0x07),
                        ExpressionValueType.Integer)
                }
            },
            { "paper", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue((args[0].AsLong() & 0x07) << 3),
                        ExpressionValueType.Integer)
                }
            },
            { "bright", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsLong() == 0 ? 0x00 : 0x40),
                        ExpressionValueType.Integer)
                }
            },
            { "flash", new []
                {
                    new FunctionEvaluator(
                        args => new ExpressionValue(args[0].AsLong() == 0 ? 0x00 : 0x80),
                        ExpressionValueType.Integer)
                }
            },
            { "attr", new []
                {
                    new FunctionEvaluator(
                        args =>
                        {
                            var ink = (byte)(args[0].AsLong() & 0x07);
                            var paper = (byte)((args[1].AsLong() & 0x07) << 3);
                            var bright = (byte)(args[2].AsLong() == 0 ? 0x00 : 0x40);
                            var flash = (byte)(args[3].AsLong() == 0 ? 0x00 : 0x80);
                            return new ExpressionValue(flash | bright | paper | ink);
                        },
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer),
                    new FunctionEvaluator(
                        args =>
                        {
                            var ink = (byte)(args[0].AsLong() & 0x07);
                            var paper = (byte)((args[1].AsLong() & 0x07) << 3);
                            var bright = (byte)(args[2].AsLong() == 0 ? 0x00 : 0x40);
                            return new ExpressionValue(bright | paper | ink);
                        },
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer),
                    new FunctionEvaluator(
                        args =>
                        {
                            var ink = (byte)(args[0].AsLong() & 0x07);
                            var paper = (byte)((args[1].AsLong() & 0x07) << 3);
                            return new ExpressionValue(paper | ink);
                        },
                        ExpressionValueType.Integer,
                        ExpressionValueType.Integer)
                }
            },
        };
    }
}