using System;
using System.Threading.Tasks;
using Ast;
using Ast.SyntaxRewriter;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task UseMethodAnyTest()
        {
            // Arrange
            var tree = @"
                    public class Foo
                    {
                        public void Any()
                        {
                            List<string> myList = new();

                            if (myList.Count > 0)
                            {
                                Console.WriteLine(""List is not empty"");
                            }

                            if (myList.Any())
                            {
                                Console.WriteLine(""List is not empty"");
                            }
                        }
                    }
            ";

            var exptectedTree = @"
                    public class Foo
                    {
                        public void Any()
                        {
                            List<string> myList = new();

                            if (myList.Any())
                            {
                                Console.WriteLine(""List is not empty"");
                            }

                            if (myList.Any())
                            {
                                Console.WriteLine(""List is not empty"");
                            }
                        }
                    }
            ";
            
            // Act
            var actualTree = await Engine.ModifySyntaxTree(tree, new UseMethodAnySyntaxRewriter());
            
            // Assert
            Assert.Equal(exptectedTree, actualTree);
        }
    }
}