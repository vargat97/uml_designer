using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UML_Diagram_Designer_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            double expect = 2.1;
            double value = 2.1;

            Assert.AreEqual(expect, value);
        }
    }
}
