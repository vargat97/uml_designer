using Microsoft.VisualStudio.TestTools.UnitTesting;
using UML_Diagram_Designer.ViewModels;
using Stylet;
using Autofac.Extras.Moq;
using Moq;
using System.Linq;
using MetaDslx.Modeling;
using UML_Diagram_Designer.Models;
using System.Windows.Controls;
using System.Windows;

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
                //Arrange
                var mutablemodel = new MutableModel();
                var immutablemodel = mutablemodel.ToImmutable();
                var eventAggregatorMock = mock.Create<EventAggregator>();
                var diagramEditorViewModel = new DiagramEditorViewModel(immutablemodel, eventAggregatorMock);

                var expected_null = 0;
                var expected = 1;
                var expected_class = "Class";

                //Act
                var actual_null = diagramEditorViewModel.ImmutableModel.Objects.Count();
                diagramEditorViewModel.CreateClass();
                var actual = diagramEditorViewModel.ImmutableModel.Objects.Count();
                var actual_metaclass = diagramEditorViewModel.ImmutableModel.Objects.First().MMetaClass;

                //Assert
                Assert.AreEqual(expected_null, actual_null,"The base model already has an object");
                Assert.AreEqual(expected, actual, "ViewModel's CreateClass could not create object");
                Assert.AreEqual(expected_class, actual_metaclass.MName, "ViewModel's CreateClass could not create Class object");
            }
        }

        [TestMethod]
        public void TestCreateInterfaeMethod()
        {
            var mutablemodel = new MutableModel();
            var immutablemodel = mutablemodel.ToImmutable();

            using (var mock = AutoMock.GetLoose())
            {
                var eventAggregatorMock = mock.Create<EventAggregator>();

                var diagramEditorViewModel = new DiagramEditorViewModel(immutablemodel, eventAggregatorMock);

                var expected_null = 0;
                var actual_null = diagramEditorViewModel.ImmutableModel.Objects.Count();

                var expected = 1;
                diagramEditorViewModel.CreateInterface();
                var actual = diagramEditorViewModel.ImmutableModel.Objects.Count();

                var expected_class = "Interface";
                var actual_metaclass = diagramEditorViewModel.ImmutableModel.Objects.First().MMetaClass;


                Assert.AreEqual(expected_null, actual_null, "The base model already has an object");
                Assert.AreEqual(expected, actual, "ViewModel's CreateInterface could not create object");
                Assert.AreEqual(expected_class, actual_metaclass.MName, "ViewModel's CreateInterface could not create Interface object");
            }

        }
        [TestMethod]
        public void TestCreateEnumerationMethod()
        {
            var mutablemodel = new MutableModel();
            var immutablemodel = mutablemodel.ToImmutable();

            using (var mock = AutoMock.GetLoose())
            {
                var eventAggregatorMock = mock.Create<EventAggregator>();

                var diagramEditorViewModel = new DiagramEditorViewModel(immutablemodel, eventAggregatorMock);

                var expected_null = 0;
                var actual_null = diagramEditorViewModel.ImmutableModel.Objects.Count();

                var expected = 1;
                diagramEditorViewModel.CreateEnum();
                var actual = diagramEditorViewModel.ImmutableModel.Objects.Count();

                var expected_class = "Enumeration";
                var actual_metaclass = diagramEditorViewModel.ImmutableModel.Objects.First().MMetaClass;


                Assert.AreEqual(expected_null, actual_null, "The base model already has an object");
                Assert.AreEqual(expected, actual, "ViewModel's CreateEnum could not create object");
                Assert.AreEqual(expected_class, actual_metaclass.MName, "ViewModel's CreateEnum could not create Enumeration object");
            }
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
