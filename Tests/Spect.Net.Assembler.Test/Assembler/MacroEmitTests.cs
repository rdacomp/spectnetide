﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class MacroEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void MacroWithNoLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .macro()
                .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0400);
        }

        [TestMethod]
        public void MacroWithLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro()
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Macros.ContainsKey("MyMacro").ShouldBeTrue();
            var def = output.Macros["MyMacro"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(0);
            def.Section.LastLine.ShouldBe(1);
            def.MacroName.ShouldBe("MYMACRO");
        }

        [TestMethod]
        public void MacroWithHangingLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: 
                    .macro()
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Macros.ContainsKey("MyMacro").ShouldBeTrue();
            var def = output.Macros["MyMacro"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(1);
            def.Section.LastLine.ShouldBe(2);
            def.MacroName.ShouldBe("MYMACRO");
        }

        [TestMethod]
        public void MacroWithArgumentsWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Macros.ContainsKey("MyMacro").ShouldBeTrue();
            var def = output.Macros["MyMacro"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(0);
            def.Section.LastLine.ShouldBe(1);
            def.MacroName.ShouldBe("MYMACRO");
        }

        [TestMethod]
        public void MacroDefWithinMacroDefIsNotAllowed()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                Nested:  .macro()
                    .endm
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0404);
        }

        [TestMethod]
        public void MacroDefWithValidParameterNamesWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    {{first}}
                    ld a,{{second}}
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MacroDefWithInvalidParameterNamesFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    {{first}}
                    ld b,{{unknown}}
                    ld {{other}},{{second}}
                    {{what}}
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(3);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0403);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0403);
            output.Errors[2].ErrorCode.ShouldBe(Errors.Z0403);
        }

        [TestMethod]
        public void MacroWithNoEndFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    ld a,b
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0401);
        }

        [TestMethod]
        [DataRow(".endm")]
        [DataRow("endm")]
        [DataRow(".ENDM")]
        [DataRow("ENDM")]
        [DataRow(".mend")]
        [DataRow("mend")]
        [DataRow(".MEND")]
        [DataRow("MEND")]
        public void EndMacroWithoutOpenTagFails(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0405);
        }
    }
}
