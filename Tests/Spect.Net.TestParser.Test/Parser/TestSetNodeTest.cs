using System.Configuration;
using System.Threading;
using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestSet;

namespace Spect.Net.TestParser.Test.Parser
{
    [TestClass]
    public class TestSetNodeTest : ParserTestBed
    {
        [TestMethod]
        public void EmptyTestSetWorks()
        {
            // --- Act
            var visitor = Parse("testset sample { source \"a.test\"; }");

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(1);

            var block = visitor.Compilation.TestSets[0];

            block.Span.StartLine.ShouldBe(1);
            block.Span.StartColumn.ShouldBe(0);
            block.Span.EndLine.ShouldBe(1);
            block.Span.EndColumn.ShouldBe(34);

            block.TestSetKeywordSpan.StartLine.ShouldBe(1);
            block.TestSetKeywordSpan.StartColumn.ShouldBe(0);
            block.TestSetKeywordSpan.EndLine.ShouldBe(1);
            block.TestSetKeywordSpan.EndColumn.ShouldBe(6);

            block.TestSetIdSpan.StartLine.ShouldBe(1);
            block.TestSetIdSpan.StartColumn.ShouldBe(8);
            block.TestSetIdSpan.EndLine.ShouldBe(1);
            block.TestSetIdSpan.EndColumn.ShouldBe(13);

            block.TestSetId.ShouldBe("sample");

            var sc = block.SourceContext;
            sc.ShouldNotBeNull();
            sc.SourceKeywordSpan.StartLine.ShouldBe(1);
            sc.SourceKeywordSpan.StartColumn.ShouldBe(17);
            sc.SourceKeywordSpan.EndLine.ShouldBe(1);
            sc.SourceKeywordSpan.EndColumn.ShouldBe(22);

            sc.SourceFileSpan.StartLine.ShouldBe(1);
            sc.SourceFileSpan.StartColumn.ShouldBe(24);
            sc.SourceFileSpan.EndLine.ShouldBe(1);
            sc.SourceFileSpan.EndColumn.ShouldBe(31);
            sc.SourceFile.ShouldBe("a.test");
        }

        [TestMethod]
        public void CallStubWorks()
        {
            // --- Act
            var visitor = Parse("testset sample { source \"a.test\"; callstub #8000; }");

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(1);
            var block = visitor.Compilation.TestSets[0];

            var cs = block.CallStub;
            cs.ShouldNotBeNull();

            cs.CallStubKeywordSpan.StartLine.ShouldBe(1);
            cs.CallStubKeywordSpan.StartColumn.ShouldBe(34);
            cs.CallStubKeywordSpan.EndLine.ShouldBe(1);
            cs.CallStubKeywordSpan.EndColumn.ShouldBe(41);
            var value = cs.Value as LiteralNode;
            value.ShouldNotBeNull();
        }

        [TestMethod]
        public void Sp48ModeWorks()
        {
            // --- Act
            var visitor = Parse("testset sample { sp48mode; source \"a.test\"; }");

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(1);
            var block = visitor.Compilation.TestSets[0];

            block.Sp48ModeSpan.StartLine.ShouldBe(1);
            block.Sp48ModeSpan.StartColumn.ShouldBe(17);
            block.Sp48ModeSpan.EndLine.ShouldBe(1);
            block.Sp48ModeSpan.EndColumn.ShouldBe(25);
            block.Sp48Mode.ShouldBeTrue();
        }

        [TestMethod]
        public void SourceContextWithNoSymbolWorks()
        {
            // --- Act
            var sc = ParseSourceContext("source \"a.test\";");

            // --- Assert

            sc.ShouldNotBeNull();
            sc.SourceKeywordSpan.StartLine.ShouldBe(1);
            sc.SourceKeywordSpan.StartColumn.ShouldBe(0);
            sc.SourceKeywordSpan.EndLine.ShouldBe(1);
            sc.SourceKeywordSpan.EndColumn.ShouldBe(5);

            sc.SourceFileSpan.StartLine.ShouldBe(1);
            sc.SourceFileSpan.StartColumn.ShouldBe(7);
            sc.SourceFileSpan.EndLine.ShouldBe(1);
            sc.SourceFileSpan.EndColumn.ShouldBe(14);
            sc.SourceFile.ShouldBe("a.test");
        }

        [TestMethod]
        public void SourceContextWithSingleSymbolWorks()
        {
            // --- Act
            var sc = ParseSourceContext("source \"a.test\" symbols Debug;");

            // --- Assert
            sc.ShouldNotBeNull();
            sc.SourceKeywordSpan.StartLine.ShouldBe(1);
            sc.SourceKeywordSpan.StartColumn.ShouldBe(0);
            sc.SourceKeywordSpan.EndLine.ShouldBe(1);
            sc.SourceKeywordSpan.EndColumn.ShouldBe(5);

            sc.SourceFileSpan.StartLine.ShouldBe(1);
            sc.SourceFileSpan.StartColumn.ShouldBe(7);
            sc.SourceFileSpan.EndLine.ShouldBe(1);
            sc.SourceFileSpan.EndColumn.ShouldBe(14);
            sc.SourceFile.ShouldBe("a.test");

            var skw = sc.SymbolsKeywordSpan;
            skw.ShouldNotBeNull();
            skw.Value.StartLine.ShouldBe(1);
            skw.Value.StartColumn.ShouldBe(16);
            skw.Value.EndLine.ShouldBe(1);
            skw.Value.EndColumn.ShouldBe(22);

            sc.Symbols.Count.ShouldBe(1);
            sc.Symbols[0].Id.ShouldBe("Debug");
        }

        [TestMethod]
        public void SourceContextWithMultipleSymbolWorks()
        {
            // --- Act
            var sc = ParseSourceContext("source \"a.test\" symbols Debug Other;");

            // --- Assert
            sc.ShouldNotBeNull();
            sc.SourceKeywordSpan.StartLine.ShouldBe(1);
            sc.SourceKeywordSpan.StartColumn.ShouldBe(0);
            sc.SourceKeywordSpan.EndLine.ShouldBe(1);
            sc.SourceKeywordSpan.EndColumn.ShouldBe(5);

            sc.SourceFileSpan.StartLine.ShouldBe(1);
            sc.SourceFileSpan.StartColumn.ShouldBe(7);
            sc.SourceFileSpan.EndLine.ShouldBe(1);
            sc.SourceFileSpan.EndColumn.ShouldBe(14);
            sc.SourceFile.ShouldBe("a.test");

            var skw = sc.SymbolsKeywordSpan;
            skw.ShouldNotBeNull();
            skw.Value.StartLine.ShouldBe(1);
            skw.Value.StartColumn.ShouldBe(16);
            skw.Value.EndLine.ShouldBe(1);
            skw.Value.EndColumn.ShouldBe(22);

            sc.Symbols.Count.ShouldBe(2);
            sc.Symbols[0].Id.ShouldBe("Debug");
            sc.Symbols[1].Id.ShouldBe("Other");
        }

        [TestMethod]
        public void DiOptionWorks()
        {
            // --- Act
            var visitor = ParseTestOptions("with di;");

            // --- Assert
            var kw = visitor.WithKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.Options.Count.ShouldBe(1);
            var option = visitor.Options[0] as DiTestOptionNode;
            option.ShouldNotBeNull();
            option.Span.StartLine.ShouldBe(1);
            option.Span.StartColumn.ShouldBe(5);
            option.Span.EndLine.ShouldBe(1);
            option.Span.EndColumn.ShouldBe(6);

            var okw = option.OptionKeywordSpan;
            okw.StartLine.ShouldBe(1);
            okw.StartColumn.ShouldBe(5);
            okw.EndLine.ShouldBe(1);
            okw.EndColumn.ShouldBe(6);
        }

        [TestMethod]
        public void EiOptionWorks()
        {
            // --- Act
            var visitor = ParseTestOptions("with ei;");

            // --- Assert
            var kw = visitor.WithKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.Options.Count.ShouldBe(1);
            var option = visitor.Options[0] as EiTestOptionNode;
            option.ShouldNotBeNull();
            option.Span.StartLine.ShouldBe(1);
            option.Span.StartColumn.ShouldBe(5);
            option.Span.EndLine.ShouldBe(1);
            option.Span.EndColumn.ShouldBe(6);

            var okw = option.OptionKeywordSpan;
            okw.StartLine.ShouldBe(1);
            okw.StartColumn.ShouldBe(5);
            okw.EndLine.ShouldBe(1);
            okw.EndColumn.ShouldBe(6);
        }

        [TestMethod]
        public void TimeoutOptionWorks()
        {
            // --- Act
            var visitor = ParseTestOptions("with timeout 1000;");

            // --- Assert
            var kw = visitor.WithKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.Options.Count.ShouldBe(1);
            var option = visitor.Options[0] as TimeoutTestOptionNode;
            option.ShouldNotBeNull();
            option.Span.StartLine.ShouldBe(1);
            option.Span.StartColumn.ShouldBe(5);
            option.Span.EndLine.ShouldBe(1);
            option.Span.EndColumn.ShouldBe(16);
            option.Expr.ShouldBeOfType<LiteralNode>();

            var okw = option.OptionKeywordSpan;
            okw.StartLine.ShouldBe(1);
            okw.StartColumn.ShouldBe(5);
            okw.EndLine.ShouldBe(1);
            okw.EndColumn.ShouldBe(11);
        }

        [TestMethod]
        public void MultipleOptionsWork1()
        {
            // --- Act
            var visitor = ParseTestOptions("with timeout 1000, di;");

            // --- Assert
            visitor.Options.Count.ShouldBe(2);
            var option1 = visitor.Options[0] as TimeoutTestOptionNode;
            option1.ShouldNotBeNull();
            option1.Expr.ShouldBeOfType<LiteralNode>();
            var option2 = visitor.Options[1] as DiTestOptionNode;
            option2.ShouldNotBeNull();
        }

        [TestMethod]
        public void MultipleOptionsWork2()
        {
            // --- Act
            var visitor = ParseTestOptions("with ei, timeout 1000;");

            // --- Assert
            visitor.Options.Count.ShouldBe(2);
            var option1 = visitor.Options[0] as EiTestOptionNode;
            option1.ShouldNotBeNull();
            var option2 = visitor.Options[1] as TimeoutTestOptionNode;
            option2.ShouldNotBeNull();
            option2.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow("testset sample { source \"a.test\"; init { A: #00; } }", "a")]
        [DataRow("testset sample { source \"a.test\"; init { B: #00; } }", "b")]
        [DataRow("testset sample { source \"a.test\"; init { C: #00; } }", "c")]
        [DataRow("testset sample { source \"a.test\"; init { D: #00; } }", "d")]
        [DataRow("testset sample { source \"a.test\"; init { E: #00; } }", "e")]
        [DataRow("testset sample { source \"a.test\"; init { H: #00; } }", "h")]
        [DataRow("testset sample { source \"a.test\"; init { L: #00; } }", "l")]
        [DataRow("testset sample { source \"a.test\"; init { a: #00; } }", "a")]
        [DataRow("testset sample { source \"a.test\"; init { b: #00; } }", "b")]
        [DataRow("testset sample { source \"a.test\"; init { c: #00; } }", "c")]
        [DataRow("testset sample { source \"a.test\"; init { d: #00; } }", "d")]
        [DataRow("testset sample { source \"a.test\"; init { e: #00; } }", "e")]
        [DataRow("testset sample { source \"a.test\"; init { h: #00; } }", "h")]
        [DataRow("testset sample { source \"a.test\"; init { l: #00; } }", "l")]
        [DataRow("testset sample { source \"a.test\"; init { xl: #00; } }", "xl")]
        [DataRow("testset sample { source \"a.test\"; init { xh: #00; } }", "xh")]
        [DataRow("testset sample { source \"a.test\"; init { yl: #00; } }", "yl")]
        [DataRow("testset sample { source \"a.test\"; init { yh: #00; } }", "yh")]
        [DataRow("testset sample { source \"a.test\"; init { XL: #00; } }", "xl")]
        [DataRow("testset sample { source \"a.test\"; init { XH: #00; } }", "xh")]
        [DataRow("testset sample { source \"a.test\"; init { YL: #00; } }", "yl")]
        [DataRow("testset sample { source \"a.test\"; init { YH: #00; } }", "yh")]
        [DataRow("testset sample { source \"a.test\"; init { ixl: #00; } }", "ixl")]
        [DataRow("testset sample { source \"a.test\"; init { ixh: #00; } }", "ixh")]
        [DataRow("testset sample { source \"a.test\"; init { iyl: #00; } }", "iyl")]
        [DataRow("testset sample { source \"a.test\"; init { iyh: #00; } }", "iyh")]
        [DataRow("testset sample { source \"a.test\"; init { IXL: #00; } }", "ixl")]
        [DataRow("testset sample { source \"a.test\"; init { IXH: #00; } }", "ixh")]
        [DataRow("testset sample { source \"a.test\"; init { IYL: #00; } }", "iyl")]
        [DataRow("testset sample { source \"a.test\"; init { IYH: #00; } }", "iyh")]
        [DataRow("testset sample { source \"a.test\"; init { IXl: #00; } }", "ixl")]
        [DataRow("testset sample { source \"a.test\"; init { IXh: #00; } }", "ixh")]
        [DataRow("testset sample { source \"a.test\"; init { IYl: #00; } }", "iyl")]
        [DataRow("testset sample { source \"a.test\"; init { IYh: #00; } }", "iyh")]
        [DataRow("testset sample { source \"a.test\"; init { i: #00; } }", "i")]
        [DataRow("testset sample { source \"a.test\"; init { I: #00; } }", "i")]
        [DataRow("testset sample { source \"a.test\"; init { r: #00; } }", "r")]
        [DataRow("testset sample { source \"a.test\"; init { R: #00; } }", "r")]
        [DataRow("testset sample { source \"a.test\"; init { bc: #00; } }", "bc")]
        [DataRow("testset sample { source \"a.test\"; init { de: #00; } }", "de")]
        [DataRow("testset sample { source \"a.test\"; init { hl: #00; } }", "hl")]
        [DataRow("testset sample { source \"a.test\"; init { sp: #00; } }", "sp")]
        [DataRow("testset sample { source \"a.test\"; init { BC: #00; } }", "bc")]
        [DataRow("testset sample { source \"a.test\"; init { DE: #00; } }", "de")]
        [DataRow("testset sample { source \"a.test\"; init { HL: #00; } }", "hl")]
        [DataRow("testset sample { source \"a.test\"; init { SP: #00; } }", "sp")]
        [DataRow("testset sample { source \"a.test\"; init { ix: #00; } }", "ix")]
        [DataRow("testset sample { source \"a.test\"; init { iy: #00; } }", "iy")]
        [DataRow("testset sample { source \"a.test\"; init { IX: #00; } }", "ix")]
        [DataRow("testset sample { source \"a.test\"; init { IY: #00; } }", "iy")]
        [DataRow("testset sample { source \"a.test\"; init { af': #00; } }", "af'")]
        [DataRow("testset sample { source \"a.test\"; init { bc': #00; } }", "bc'")]
        [DataRow("testset sample { source \"a.test\"; init { de': #00; } }", "de'")]
        [DataRow("testset sample { source \"a.test\"; init { hl': #00; } }", "hl'")]
        [DataRow("testset sample { source \"a.test\"; init { AF': #00; } }", "af'")]
        [DataRow("testset sample { source \"a.test\"; init { BC': #00; } }", "bc'")]
        [DataRow("testset sample { source \"a.test\"; init { DE': #00; } }", "de'")]
        [DataRow("testset sample { source \"a.test\"; init { HL': #00; } }", "hl'")]
        public void InitAssignmentWithRegSpecWorks(string code, string reg)
        {
            // --- Act
            var visitor = Parse(code);

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(1);
            var ts = visitor.Compilation.TestSets[0];
            ts.Init.ShouldNotBeNull();
            ts.Init.Assignments.Count.ShouldBe(1);
            var asg = ts.Init.Assignments[0] as RegisterAssignmentNode;
            asg.ShouldNotBeNull();
            asg.RegisterName.ShouldBe(reg);
            asg.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow("testset sample { source \"a.test\"; init { .z; } }", "z")]
        [DataRow("testset sample { source \"a.test\"; init { .nz; } }", "nz")]
        [DataRow("testset sample { source \"a.test\"; init { .c; } }", "c")]
        [DataRow("testset sample { source \"a.test\"; init { .nc; } }", "nc")]
        [DataRow("testset sample { source \"a.test\"; init { .pe; } }", "pe")]
        [DataRow("testset sample { source \"a.test\"; init { .po; } }", "po")]
        [DataRow("testset sample { source \"a.test\"; init { .p; } }", "p")]
        [DataRow("testset sample { source \"a.test\"; init { .m; } }", "m")]
        [DataRow("testset sample { source \"a.test\"; init { .n; } }", "n")]
        [DataRow("testset sample { source \"a.test\"; init { .a; } }", "a")]
        [DataRow("testset sample { source \"a.test\"; init { .h; } }", "h")]
        [DataRow("testset sample { source \"a.test\"; init { .nh; } }", "nh")]
        [DataRow("testset sample { source \"a.test\"; init { .3; } }", "3")]
        [DataRow("testset sample { source \"a.test\"; init { .n3; } }", "n3")]
        [DataRow("testset sample { source \"a.test\"; init { .5; } }", "5")]
        [DataRow("testset sample { source \"a.test\"; init { .n5; } }", "n5")]
        [DataRow("testset sample { source \"a.test\"; init { .Z; } }", "z")]
        [DataRow("testset sample { source \"a.test\"; init { .NZ; } }", "nz")]
        [DataRow("testset sample { source \"a.test\"; init { .C; } }", "c")]
        [DataRow("testset sample { source \"a.test\"; init { .NC; } }", "nc")]
        [DataRow("testset sample { source \"a.test\"; init { .PE; } }", "pe")]
        [DataRow("testset sample { source \"a.test\"; init { .PO; } }", "po")]
        [DataRow("testset sample { source \"a.test\"; init { .P; } }", "p")]
        [DataRow("testset sample { source \"a.test\"; init { .M; } }", "m")]
        [DataRow("testset sample { source \"a.test\"; init { .N; } }", "n")]
        [DataRow("testset sample { source \"a.test\"; init { .A; } }", "a")]
        [DataRow("testset sample { source \"a.test\"; init { .H; } }", "h")]
        [DataRow("testset sample { source \"a.test\"; init { .NH; } }", "nh")]
        [DataRow("testset sample { source \"a.test\"; init { .3; } }", "3")]
        [DataRow("testset sample { source \"a.test\"; init { .N3; } }", "n3")]
        [DataRow("testset sample { source \"a.test\"; init { .5; } }", "5")]
        [DataRow("testset sample { source \"a.test\"; init { .N5; } }", "n5")]
        public void InitAssignmentWithFlagWorks(string code, string flag)
        {
            // --- Act
            var visitor = Parse(code);

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(1);
            var ts = visitor.Compilation.TestSets[0];
            ts.Init.ShouldNotBeNull();
            ts.Init.Assignments.Count.ShouldBe(1);
            var asg = ts.Init.Assignments[0] as FlagAssignmentNode;
            asg.ShouldNotBeNull();
            asg.FlagName.ShouldBe(flag);
        }

        [TestMethod]
        public void InitAssignmentWithMemAssignmentWorks1()
        {
            // --- Act
            var visitor = Parse("testset sample { source \"a.test\"; init { [#1234]: myMem; } }");

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(1);
            var ts = visitor.Compilation.TestSets[0];
            ts.Init.ShouldNotBeNull();
            ts.Init.Assignments.Count.ShouldBe(1);
            var asg = ts.Init.Assignments[0] as MemoryAssignmentNode;
            asg.ShouldNotBeNull();
            asg.Address.ShouldBeOfType<LiteralNode>();
            asg.Value.ShouldBeOfType<IdentifierNode>();
            asg.Length.ShouldBeNull();
        }

        [TestMethod]
        public void InitAssignmentWithMemAssignmentWorks2()
        {
            // --- Act
            var visitor = Parse("testset sample { source \"a.test\"; init { [#1234]: myMem:100; } }");

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(1);
            var ts = visitor.Compilation.TestSets[0];
            ts.Init.ShouldNotBeNull();
            ts.Init.Assignments.Count.ShouldBe(1);
            var asg = ts.Init.Assignments[0] as MemoryAssignmentNode;
            asg.ShouldNotBeNull();
            asg.Address.ShouldBeOfType<LiteralNode>();
            asg.Value.ShouldBeOfType<IdentifierNode>();
            asg.Length.ShouldBeOfType<LiteralNode>();
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private static SourceContextNode ParseSourceContext(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.sourceContext();
            var visitor = new Z80TestVisitor();
            var result = (SourceContextNode)visitor.VisitSourceContext(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return result;
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private static TestOptionsNode ParseTestOptions(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.testOptions();
            var visitor = new Z80TestVisitor();
            var result = (TestOptionsNode)visitor.VisitTestOptions(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return result;
        }
    }
}
