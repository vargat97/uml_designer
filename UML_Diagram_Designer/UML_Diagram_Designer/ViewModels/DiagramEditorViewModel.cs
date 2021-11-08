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
        private IEventAggregator _eventAggregator;
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

        public void ChangeObjectNameParameter(string parameter)
        {
            if(parameter == "enum")
            {

            }
        }
        public void ChangeObjectName(TextBox sender, System.EventArgs e)
        {
            var immutableObject = this.DetailsObject;
            var immutableModel = this._functions.ModifyObjectName(immutableObject, sender.Text);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        // TODO :Nodelayout, EdgeLayout refactor to detailsobject, and remove that function
        public void ChangeName(TextBox sender, System.EventArgs e)
        {

            var layout = (NodeLayout)(sender.DataContext);

            var immutableObject = (ImmutableObject)(layout.NodeObject);
            var immutableModel = this._functions.ModifyObjectName(immutableObject, sender.Text);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        //TODO: create a uniform remove function. Questions: what to do with the parameters of the operations and etc..
        public void RemoveFromModel()
        {

        }

        public void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                this.ChangeName((TextBox)sender, e);
            }
        }








        public void ChangeVisibility(ComboBox sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var addedItem = e.AddedItems[0];
            var visibilityKind = (VisibilityKind)addedItem;
            ImmutableObject immutableObject = null;
            if (NodeLayout != null) immutableObject = (ImmutableObject)NodeLayout.NodeObject;
            else if (EdgeLayout != null) immutableObject = (ImmutableObject)EdgeLayout.EdgeObject;
            else if  (DetailsObject != null) immutableObject = DetailsObject;

            if (immutableObject == null) return;
            this._functions = new Functions(this._immutableModel);
            var immutableModel = this._functions.ModifyObjectVisivility(immutableObject, visibilityKind);

            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        public void AddParameterToOperation(Button sender,RoutedEventArgs e)
        {
            var operationObject = (ImmutableObject)(sender.DataContext);
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();
            var parametertype = operationObject.MParent;
            var immutableModel = this._functions.AddNewParameterToOperation(name, parametertype, operationObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }

        public void RemoveProperty(Button sender, RoutedEventArgs e)
        {
            var asd = sender.DataContext;
            var immutableModel = this._functions.RemovePropertyObject((Property)asd);

            this._immutableModel = immutableModel;
            DetailsObject = null;
            this.PublishEvent();
        }

        public void RemoveOperation(Button sender, RoutedEventArgs e)
        {
            var asd = sender.DataContext;
            var immutableModel = this._functions.RemoveOperationObject((Operation)asd);

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

        public void IsAbstractCheck(CheckBox sender, RoutedEventArgs e)
        {
            if(NodeLayout == null) return ;
            var isAbstract = ((Classifier)NodeLayout.NodeObject).IsAbstract;
            if(isAbstract == sender.IsChecked) return;
            var immutableModel = this._functions.SetObjectToAbstract((ImmutableObject)NodeLayout.NodeObject);
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }
        public void AddAttributeToClassifier(object sender, RoutedEventArgs e)
        {
            var immutableModel = this.CreateProperty();
            this._immutableModel = immutableModel;
            this.PublishEvent();
        }




        public void CreateClass()
        {
            var immutableModel = this._createClass();
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

        public void CreateEnumLiteral()
        {
            var immutableModel = this._functions.CreateEnumLiteral((ImmutableObject)NodeLayout.NodeObject, "Asd");
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

        private ImmutableModel CreateProperty()
        {

            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();

            var mutableModel = this._immutableModel.ToMutable();
            this._factory = new UmlFactory(mutableModel);
            var propertyBuilder = this._factory.Property();
            propertyBuilder.MName = name;
            mutableModel.ResolveObject(propertyBuilder.MId);
            var propertyObject = propertyBuilder.ToImmutable();
            var classObject = ((Classifier)NodeLayout.NodeObject).ToMutable();
            var classBuilder = (ClassBuilder)classObject;
            propertyBuilder.SetClassLazy(propertyBuilder1 => classBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(propertyObject), propertyBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(classObject), classBuilder);


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
