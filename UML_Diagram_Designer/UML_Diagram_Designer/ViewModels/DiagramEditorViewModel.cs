using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using RandomNameGeneratorLibrary;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UML_Diagram_Designer.Models;

namespace UML_Diagram_Designer.ViewModels
{
    public class DiagramEditorViewModel : Screen
    {

        private ImmutableObject _relationShipObject;
        private List<ImmutableObject> _modelstypes;
        private readonly IEventAggregator _eventAggregator;
        private NodeLayout _nodeLayout;
        private EdgeLayout _edgeLayout;
        private ImmutableObject _detailsObject;
        private ImmutableModel _immutableModel;
        private UmlFactory _factory;
        private Functions _functions;

        public List<ImmutableObject> ModelsTypes
        {
            get { return this._modelstypes; }
            set
            {
                SetAndNotify(ref this._modelstypes, value);
            }
        }
        public NodeLayout NodeLayout
        {
            get { return this._nodeLayout; }
            set
            {
                SetAndNotify(ref this._nodeLayout, value);
            }
        }
        public EdgeLayout EdgeLayout
        {
            get { return this._edgeLayout; }
            set
            {
                SetAndNotify(ref this._edgeLayout, value);
            }
        }
        public ImmutableObject DetailsObject
        {
            get { return this._detailsObject; }
            set
            {
                SetAndNotify(ref this._detailsObject, value);
            }
        }

        public ImmutableModel ImmutableModel
        {
            get { return this._immutableModel; }
            set { this._immutableModel = value; }
        }

        public ImmutableObject RelationShipObject
        {
            get { return this._relationShipObject; }
            set
            {
                SetAndNotify(ref this._relationShipObject, value);
                this.NotifyOfPropertyChange(nameof(this.IsCreateRealtionShip));
            }
        }
        public DiagramEditorViewModel(ImmutableModel immutableModel, IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            this._immutableModel = immutableModel;
            this._functions = new Functions(_immutableModel);
            this.FillModelsTypesCollection();
        }


        public bool IsCreateRealtionShip
        {
            get { return RelationShipObject != null; }

            set
            {
                if (!value)
                {
                    RelationShipObject = null;
                }
            }
        }

        public void PublishEvent()
        {
            var immutableModelChangedEvnt = new ImmutableModelChangedEvent(this._immutableModel);
            this._eventAggregator.Publish(immutableModelChangedEvnt);
        }
        /// <summary>
        /// Changes the enum literals name
        /// </summary>
        /// <param name="parameter">Enum literal's neew name</param>
        /// <returns>No return value</returns>
        public void ChangeEnumLiteralName(string parameter)
        {
            //TODO
        }

        /// <summary>
        /// Change the selected immutableObject's name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeObjectName(TextBox sender, System.EventArgs e)
        {
            var immutableObject = this.DetailsObject;
            var immutableModel = this._functions.ModifyObjectName(immutableObject, sender.Text);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        /// <summary>
        /// Change The name of the Nodelayout's object
        /// </summary>
        /// <param name="sender">Textbox, contains the new name of the object</param>
        /// <param name="e">EventArgs</param>
        /// <returns>No returns value</returns>
        public void ChangeName(TextBox sender,EventArgs e)
        { 
            var immutableObject = (ImmutableObject)(NodeLayout.NodeObject);
            var immutableModel = this._functions.ModifyObjectName(immutableObject, sender.Text);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        /// <summary>
        /// Remove the ImmutableObject of the selected ClassifierLayout from the model
        /// Removes all the selected object relevant objects too
        /// Than Publish an event to notify the subscribers to refresh their states
        /// </summary>
        /// <param>No parameter value</param>
        /// <returns>No return value</returns>
        public void RemoveClassFromModel()
        {
            var immutableObject = (ImmutableObject)this.NodeLayout.NodeObject;
            var immutableModel = this._functions.RemoveClassObjectFromModel((Class)immutableObject);
            this._immutableModel = immutableModel;
            NodeLayout = null;
            this.PublishEvent();
        }
        /// <summary>
        /// Removes the selected interfacerealization from the Model
        /// Removes the realized interface's attributes and operations
        /// </summary>
        /// <param name="sender">Neccesary parameter for the click action</param>
        /// <param name="e">Neccesary parameter for the click action</param>
        /// <returns>No return value</returns>
        public void RemoveInterfaceRealizationFromModel(object sender, RoutedEventArgs e)
        {
            var interfaceRealization = (InterfaceRealization)EdgeLayout.EdgeObject;
            var immutableModel = this._functions.RemveInterfaceRealizationFromModel(interfaceRealization);
            this._immutableModel = immutableModel;
            EdgeLayout = null;
            this.PublishEvent();
        }
        /// <summary>
        /// Removes the selected Generalization from the Model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveGeneralizationFromModel(object sender, RoutedEventArgs e)
        {
            var generalization = (Generalization)EdgeLayout.EdgeObject;
            var immutableModel = this._functions.RemoveGeneralizationFromModel(generalization);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        /// <summary>
        /// Removes the selected Dependency from the model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveDependencyFromModel(object sender, RoutedEventArgs e)
        {
            var dependecy = (Dependency)EdgeLayout.EdgeObject;
            var immutableModel = this._functions.RemoveDependencyFromModel(dependecy);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        /// <summary>
        /// Removes the selected EnumerationLiteral from the model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveEnumerationLiteral(object sender, RoutedEventArgs e)
        {
            var literal = (EnumerationLiteral)DetailsObject;
            var immutableModel = this._functions.RemoveEnumerationLiteralFromModel(literal);
            DetailsObject = null;
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        //TODO
        public void RemoveAssociationFromModel(object sender, RoutedEventArgs e)
        {
            var association = (Association)EdgeLayout.EdgeObject;
            var immutableModel = this._functions.RemoveAssociationFromModel(association);
            this._immutableModel = immutableModel;
            EdgeLayout = null;
            this.PublishEvent();
        }

        /// <summary>
        /// Changes the visibility of the immutableObject</summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        /// <returns>No return value</returns>
        public void ChangeVisibility(ComboBox sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var addedItem = e.AddedItems[0];
            var visibilityKind = (VisibilityKind)addedItem;
            ImmutableObject immutableObject = null;
            if (NodeLayout != null) immutableObject = (ImmutableObject)NodeLayout.NodeObject;
            else if (EdgeLayout != null) immutableObject = (ImmutableObject)EdgeLayout.EdgeObject;
            else if (DetailsObject != null) immutableObject = DetailsObject;

            if (immutableObject == null) return;
            this._functions = new Functions(this._immutableModel);
            var immutableModel = this._functions.ModifyObjectVisivility(immutableObject, visibilityKind);

            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        /// <summary>
        /// Checks if the keydown button was an Enter, and if yes, call ChangeName function </summary>
        /// <param name="sender"></param>
        /// <param name="e">KeyEventArgs</param>
        /// <returns>No return value</returns>
        public void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if(NodeLayout != null || EdgeLayout != null )this.ChangeName((TextBox)sender,e);
                else { this.ChangeObjectName((TextBox)sender, e); }
            }
        }
        /// <summary>
        /// Removes the iterface object, its childs and all relevant interfacerealizations from the model
        /// </summary>
        /// <param name="interface">Interface, what we would like to remove</param>
        /// <returns>No return value</returns>
        public void RemoveInterfaceFromModel()
        {
            var immutableObject = (ImmutableObject)this.NodeLayout.NodeObject;
            var immutableModel = this._functions.RemoveInterfaceFromModel((Interface)immutableObject);
            this._immutableModel = immutableModel;
            EdgeLayout = null;
            this.PublishEvent();
        }
        /// <summary>
        /// Removes the enum object and its childs from the model
        /// </summary>
        public void RemoveEnumFromTheModel()
        {
            var immutableObject = (ImmutableObject)this.NodeLayout.NodeObject;
            var immutableModel = this._functions.RemoveEnumFromModel((Enumeration)immutableObject);
            this._immutableModel = immutableModel;
            NodeLayout = null;
            this.PublishEvent();

        }

        /// <summary>
        /// Removes the selected Parameter from the model
        /// </summary>
        public void RemoveParameterFromModel()
        {
            var immutableObject = DetailsObject;
            var immutableModel = this._functions.RemoveParameterFromModel((Parameter)immutableObject);
            this._immutableModel = immutableModel;
            DetailsObject = null;
            this.PublishEvent();
        }

        /// <summary>
        /// If the nodelayout's nodeobject is not abstract, than set it as abstarct
        /// </summary>
        /// <param name="sender">Combobox</param>
        /// <param name="e">RoutedEventArgs</param>
        /// <returns>No return value</returns>
        public void IsAbstractCheck(CheckBox sender, RoutedEventArgs e)
        {
            if (NodeLayout == null) return;
            var isAbstract = ((Classifier)NodeLayout.NodeObject).IsAbstract;
            if (isAbstract == sender.IsChecked) return;
            var immutableModel = this._functions.SetObjectToAbstract((ImmutableObject)NodeLayout.NodeObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        /// <summary>
        /// Adds a new parameter to the given operation with a random name, and a type of the operation's classifier.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddParameterToOperation(Button sender,RoutedEventArgs e)
        {
            var operationObject = DetailsObject;
            var immutableModel = this._functions.AddNewParameterToOperation(operationObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        /// <summary>
        /// Opens a new "tab" on diagramEditorView, where user can edit the selected Paramater
        /// </summary>
        /// <param name="parameter"></param>
        public void OpenEditParameter(object parameter)
        {
            DetailsObject = (Parameter)parameter;
        }

        /// <summary>
        /// Removes the given property from the model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveProperty(Button sender, RoutedEventArgs e)
        {
            var immutableObject = DetailsObject;
            var immutableModel = this._functions.RemovePropertyObject((Property)immutableObject);
            this._immutableModel = immutableModel;
            DetailsObject = null;
            this.PublishEvent();
        }

        /// <summary>
        /// Removes the operation from the model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveOperation(Button sender, RoutedEventArgs e)
        {
            var operationObject = DetailsObject;
            var immutableModel = this._functions.RemoveOperationObject((Operation)operationObject);
            DetailsObject = null;
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }



        public void ChangeObjectType(ComboBox sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var addedItem = (ImmutableObject)e.AddedItems[0];
            ImmutableObject immutableObject = null;
            if (NodeLayout != null) immutableObject = (ImmutableObject)NodeLayout.NodeObject;
            else if (EdgeLayout != null) immutableObject = (ImmutableObject)EdgeLayout.EdgeObject;
            else if (DetailsObject != null) immutableObject = DetailsObject;

            if (immutableObject == null) return;
            this._functions = new Functions(this._immutableModel);
            var immutableModel = this._functions.ModifyObjectType(immutableObject, addedItem);

            this._immutableModel = immutableModel;
            this.PublishEvent();
        }











        public void CreateAssociation(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = btn.Background == SystemColors.ControlBrush ? Brushes.Blue : SystemColors.ControlBrush;
            this._relationShipObject = this._functions.CreateAssociation();

        }
        public void CreateInterfaceRealization(object sender, RoutedEventArgs e)
        {
            RelationShipObject = this._functions.CreateInterfaceRealization();
        }

        public void CreateDependency(object sender, RoutedEventArgs e)
        {
            RelationShipObject = this._functions.CreateDependency();
        }

        public void CreateGeneralization(object sender, RoutedEventArgs e)
        {
            RelationShipObject = this._functions.CreateGeneralization();
        }

        /// <summary>
        /// Adds a new attribute to the class with a random name
        /// User can edit that by clickink the new attribute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddAttributeToClass(object sender, RoutedEventArgs e)
        {
            var classObject = (Class)NodeLayout.NodeObject;
            var immutableModel = this._functions.CreateAttributeToClass(classObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        /// <summary>
        /// Adds a new operation to the class with a random name
        /// User can edit the nam later by clicking the new operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddOperationToClass(object sender, RoutedEventArgs e)
        {
            var classObject = (Class)NodeLayout.NodeObject;
            var immutableModel = this._functions.CreateOperationToClass(classObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        /// <summary>
        /// Adds a new attribute to the interface with a random name
        /// User can edit the name of the interface later by clicking the new property's name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddAttributeToInterface(object sender, RoutedEventArgs e)
        {
            var interfaceObject = (Interface)NodeLayout.NodeObject;
            var immutableModel = this._functions.CreateAttributeToInterface(interfaceObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        /// <summary>
        /// Adds a new operation to the interface with a random name
        /// User can edit the name of the interface later by clicking the new operation's name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddOperationToInterface(object sender, RoutedEventArgs e)
        {
            var interfaceObject = (Interface)NodeLayout.NodeObject;
            var immutableModel = this._functions.CreateOperationToInterface(interfaceObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        /// <summary>
        /// Adds a new enumerationLiteral to the enumeration with a random name
        /// User can edit the name of the enumrationliteral by clicking the new literal's name
        /// </summary>
        public void AddEnumLiteralToEnum()
        {
            var immutableObject = (ImmutableObject)NodeLayout.NodeObject;
            var immutableModel = this._functions.CreateEnumLiteralToEnumeration((Enumeration)immutableObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        public void OpenEditEnumLiteral()
        {
            //TODO: OPEN the selected enumliteral
        }

        public void CreateClass()
        {
            var immutableModel = this._functions.CreateClass();
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        public void CreateInterface()
        {
            var immutableModel = this._createInterface();
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        public void CreateEnum()
        {
            var immutableModel = this._createEnum();
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }



        private ImmutableModel _createEnum()
        {
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();

            var mutableModel = this._immutableModel.ToMutable();
            this._factory = new UmlFactory(mutableModel);
            var enumerationBuilder = this._factory.Enumeration();
            enumerationBuilder.MName = name;
            mutableModel.ResolveObject(enumerationBuilder.MId);
            return mutableModel.ToImmutable();
        }

        private ImmutableModel _createInterface()
        {
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();

            var mutableModel = this._immutableModel.ToMutable();
            this._factory = new UmlFactory(mutableModel);
            var interfaceBuilder = this._factory.Interface();
            interfaceBuilder.MName = name;
            mutableModel.ResolveObject(interfaceBuilder.MId);
            return mutableModel.ToImmutable();
        }

        private ImmutableModel _createClass()
        {
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();


             var mutableModel = this._immutableModel.ToMutable();
            this._factory = new UmlFactory(mutableModel);
             var classBuilder = this._factory.Class();
             classBuilder.MName = name;
             mutableModel.ResolveObject(classBuilder.MId);
            return mutableModel.ToImmutable();

        }


        private void FillModelsTypesCollection()
        {
            var my_hash = new HashSet<ImmutableObject>();
            foreach(var model_object in this._immutableModel.Objects)
            {
                if(model_object.MType != null)
                {
                    my_hash.Add(model_object.MType);
                }
            }
            ModelsTypes = my_hash.ToList();
        }

    } 
}
