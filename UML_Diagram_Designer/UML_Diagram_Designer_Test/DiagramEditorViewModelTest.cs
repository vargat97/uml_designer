using Microsoft.VisualStudio.TestTools.UnitTesting;
using UML_Diagram_Designer.ViewModels;
using Stylet;
using Autofac.Extras.Moq;
using Moq;
using System.Linq;
using MetaDslx.Modeling;

namespace UML_Diagram_Designer_Test
{
    [TestClass]
    public class DiagramEditorViewModelTest
    {
        [TestMethod]
        public void TestCreateClassMethod_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var immutableModel = GetModelSample();
                var eventAggregatorMock = mock.Create<EventAggregator>();

                var diagramEditorViewModelMock = mock.Mock<DiagramEditorViewModel>();
                diagramEditorViewModelMock.CreateClass();

                var expected = 1;
                var actual = diagramEditorViewModelMock.ImmutableModel.Objects.Count();

                //var expected = GetModelSample().ToMutable().Name;
                //var actual = diagramEditorViewModelMock.ImmutableModel.ToMutable().Name;
                Assert.AreEqual(expected, actual);

            }
        }
        private static ImmutableModel GetModelSample()
        {
            var mutableModel = new MutableModel();
            mutableModel.Name = "UnitTest";
            return mutableModel.ToImmutable();
        }

        [TestMethod]
        public void TestCreateInterfaeMethod()
        {

        }
        [TestMethod]
        public void TestCreateEnumerationMethod()
        {

        }
        [TestMethod]
        public void TestCreateAttributeMethod()
        {

        }
        [TestMethod]
        public void TestCreateOperationMethod()
        {

        }
        [TestMethod]
        public void TestCreateParameterMethod()
        {

        }
        [TestMethod]
        public void TestCreateEnumerationLiterralMethod()
        {

        }
        [TestMethod]
        public void TestCreateAssociationMethod()
        {

        }
        [TestMethod]
        public void TestCreateDependencyMethod()
        {

        }
        [TestMethod]
        public void TestCreateGeneralizationMethod()
        {

        }
        [TestMethod]
        public void TestCreateInterfaceRealizationMethod()
        {

        }
        [TestMethod]
        public void TestModifyObjectNameMethod()
        {

        }
        [TestMethod]
        public void TestModifyObjectVisibilityMethod()
        {
        }
        [TestMethod]
        public void TestRemoveClassMethod()
        {

        }
        [TestMethod]
        public void TestRemoveInterfaceMethod()
        {

        }
        [TestMethod]
        public void TestRemoveEnumerationMethod()
        {

        }
        [TestMethod]
        public void TestRemoveAttributeMethod()
        {

        }
        [TestMethod]
        public void TestRemoveOperationethod()
        {

        }
        [TestMethod]
        public void TestRemoveParameterMethod()
        {

        }
        [TestMethod]
        public void TestRemoveEnumrationLiteralMethod()
        {

        }
        [TestMethod]
        public void TestRemoveAssociationMethod()
        {

        }
        [TestMethod]
        public void TestRemoveDependencyMethod()
        {

        }
        [TestMethod]
        public void TestRemoveGeneralizationMethod()
        {

        }
        public void TestRemoveInterfaceRealizationMethod()
        {

        }
    }
}
