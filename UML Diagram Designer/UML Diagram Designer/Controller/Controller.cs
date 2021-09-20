using DiagramDesigner.Model;
using MetaDslx.GraphViz;
using UML_Diagram_Designer.Controller;

namespace DiagramDesigner.Controller
{
    public class Controller
    {
        private  UMLModel model;
        private  GraphLayoutModel graphLayoutModel;
        private UmlFactory factory;

        public Controller()
        {

        }
        public UMLModel GetUMLModel()
        {
            return this.model;
        }
        public GraphLayoutModel GetGraphLayoutModel()
        {
            return this.graphLayoutModel;
        }
        public void SetModel(UMLModel model)
        {
            this.model = model;
        }
        public void SetGraphLayoutModel(GraphLayoutModel graphLayoutModel)
        {
            this.graphLayoutModel = graphLayoutModel;
        }

        public void SetFactory()
        {
            if(this.model != null)
            {
                factory = new UmlFactory(this.model.Model.ToMutable());
            }
        }

        public GraphLayout OpenFileClickControl(string fileName)
        {
            this.model = new UMLModel(fileName);
            this.factory = new UmlFactory(this.model.Model.ToMutable());
            this.graphLayoutModel = new GraphLayoutModel(new MetaDslx.GraphViz.GraphLayout("dot"), this.model);
            this.graphLayoutModel.LoadGraphLayoutModel();

            return this.graphLayoutModel.GraphLayout;
        }
        public bool SaveFileClickControl(string fileName)
        {

            if (this.model != null)
            {
                UmlXmiSerializer serializer = new UmlXmiSerializer();
                serializer.WriteModelToFile(fileName, model.Model);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Modify the first ImmutableObject parameter's name to the second parameter.
        /// </summary>
        /// <param name="obj">The ImmutableObject object, which name's you want to modify</param>
        /// <param name="newName">The ImmutableObject object's new name</param>
        public void ModifyObjectName(ImmutableObject obj, string newName)
        {
            if (this.model.Model.ContainsObject(obj))
            {

                var mutableModel = this.model.Model.ToMutable();
                var mutableObject = mutableModel.GetObject(obj.ToMutable());
                var namedElement = (MetaDslx.Languages.Uml.Model.NamedElement)obj;

                var namedElementBuilder = namedElement.ToMutable();
                namedElementBuilder.SetNameLazy(n => newName);
                mutableModel.MergeObjects(mutableModel.GetObject(mutableObject), namedElementBuilder);
                this.model.Model = mutableModel.ToImmutable();
                this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
            }
        }
        /// <summary>
        /// Modify the first ImmutableObject parameter's visibiliy to the second parameter.
        /// </summary>
        /// <param name="obj">The ImmutableObject object, which visibility's you want to modify</param>
        /// <param name="kind"></param>
        public void ModifyObjectVisivility(ImmutableObject obj, VisibilityKind kind)
        {
            if (this.model.Model.ContainsObject(obj))
            {
                var mutableObject = obj.ToMutable();
                var mutableModel = this.model.Model.ToMutable();
                var namedElemen = (MetaDslx.Languages.Uml.Model.NamedElement)obj;
                var namedElementBuilder = namedElemen.ToMutable();
                namedElementBuilder.SetVisibilityLazy(namedElementBuilder1 => kind);
     
                mutableModel.MergeObjects(mutableObject, namedElementBuilder);
                this.model.Model = mutableModel.ToImmutable();
            }
   
        }
      public void RemoveObject(ImmutableObject immutableObject)
        {
            var mutableModel = this.model.Model.ToMutable();
            var mutableObject = immutableObject.ToMutable();
            if (mutableModel.RemoveObject(this.model.Model.GetObject(mutableObject).ToMutable()))
            {
                this.model.Model = mutableModel.ToImmutable();
                this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
            }
        }

        //CLASS related methods//////
                 // CREATE methods
        public ImmutableObject CreateClass(string className)
        {
            var mutableModel = this.model.Model.ToMutable();
            var classBuilder = this.factory.Class();
            classBuilder.MName = className;
            mutableModel.ResolveObject(classBuilder.MId);
            this.model.Model = mutableModel.ToImmutable();
            this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
            return classBuilder.ToImmutable();
        }


      public ImmutableObject CreateOperation(string operationName)
      {
          var mutableModel = this.model.Model.ToMutable();
          var operationBuilder = this.factory.Operation();
          operationBuilder.MName = operationName;

          mutableModel.ResolveObject(operationBuilder.MId);

          this.model.Model = mutableModel.ToImmutable();
          return operationBuilder.ToImmutable();
      }
        
      public ImmutableObject CreateProperty(string propertyName)
      {
          var mutableModel = this.model.Model.ToMutable();

          var propertyBuilder = this.factory.Property();
          propertyBuilder.MName = propertyName;
          mutableModel.ResolveObject(propertyBuilder.MId);
          this.model.Model = mutableModel.ToImmutable();
          return propertyBuilder.ToImmutable();
      }
               // END Class related CREATE methods
               //START CLASS related SET methods

        /*
      public void SetClassToAbstract(ImmutableObject classObject)
      {
          var mutableModel = this.Model.ToMutable();

          if (!mutableModel.ContainsObject(classObject)) return;

          var classBuilderObject = (ClassBuilder)classObject.ToMutable();

          classBuilderObject.SetIsAbstractLazy(classBuilderobj => true);

          mutableModel.MergeObjects(mutableModel.GetObject(classObject), classBuilderObject);
          this.Model = mutableModel.ToImmutable();
      }
        */
      public void SetOperationToClass(ImmutableObject classObject, ImmutableObject operationObject)
      {
          var mutableModel = this.model.Model.ToMutable();

          if (!(mutableModel.ContainsObject(classObject) && mutableModel.ContainsObject(operationObject))) return;

          var classBuilder = (ClassBuilder)classObject.ToMutable();
          var operationBuilder = (OperationBuilder)operationObject.ToMutable();

          operationBuilder.SetClassLazy(operationBuilder1 => classBuilder);

          mutableModel.MergeObjects(mutableModel.GetObject(operationObject), operationBuilder);
          mutableModel.MergeObjects(mutableModel.GetObject(classObject), classBuilder);

          this.model.Model = mutableModel.ToImmutable();
      }
        public ImmutableObject CreateInterface(string interfaceName)
        {
            var mutableModel = this.model.Model.ToMutable();

            var interfaceBuilder = this.factory.Interface();
            interfaceBuilder.MName = interfaceName;
            mutableModel.ResolveObject(interfaceBuilder.MId);
            this.model.Model = mutableModel.ToImmutable();
            this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
            return interfaceBuilder.ToImmutable();
        }
        public ImmutableObject CreateEnum(string enumName)
        {
            var mutableModel = this.model.Model.ToMutable();
            var enumerationBuilder = this.factory.Enumeration();
            enumerationBuilder.MName = enumName;

            mutableModel.ResolveObject(enumerationBuilder.MId);

            this.model.Model = mutableModel.ToImmutable();
            this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
            return enumerationBuilder.ToImmutable();
        }
        
    public void SetObjectType(ImmutableObject immutableObject, ImmutableObject typeObject)
        {
            var mutableModel = this.model.Model.ToMutable();
            var typedElementBuilder = (TypedElementBuilder)immutableObject.ToMutable();
            var typeBuilder = (TypeBuilder)typeObject.ToMutable();

            typedElementBuilder.SetTypeLazy(t => typeBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(immutableObject), typedElementBuilder);

            this.model.Model = mutableModel.ToImmutable();

            this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
        }
      public void SetPropertyToClass(ImmutableObject classObject, ImmutableObject propertyObject)
      {
          var mutableModel = this.model.Model.ToMutable();

          if (!(mutableModel.ContainsObject(classObject) && mutableModel.ContainsObject(propertyObject))) return;

          var classBuilder = (ClassBuilder)classObject.ToMutable();
          var propertyBuilder = (PropertyBuilder)propertyObject.ToMutable();

          propertyBuilder.SetClassLazy(propertyBuilder1 => classBuilder);
          mutableModel.MergeObjects(mutableModel.GetObject(propertyObject), propertyBuilder);
          mutableModel.MergeObjects(mutableModel.GetObject(classObject), classBuilder);

          this.model.Model = mutableModel.ToImmutable();
          this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
        }
        /*
      public void RemoveOperationFromClass(ImmutableObject operationObject)
      {
          this.RemoveObject(operationObject);
      }
      public void RemovePropertyFromClass(ImmutableObject propertyObject)
      {
          this.RemoveObject(propertyObject);
      }
               //END CLASS related SET methods

      //END CLASS realted methods

      //INTERFACE related methods
           //CREATE methods

              //END INTERFACE related CREATE methods

              //START INTERFACE related SET methods
      public void SetOperationToInterface(ImmutableObject interfaceObject, ImmutableObject operationObject)
      {
          var mutableModel = this.Model.ToMutable();

          var operationBuilder = (OperationBuilder)operationObject.ToMutable();
          var interfaceBuilder = (InterfaceBuilder)interfaceObject.ToMutable();

          operationBuilder.SetInterfaceLazy(opertaionBuilder => interfaceBuilder);

          mutableModel.MergeObjects(this.Model.GetObject(operationObject).ToMutable(), operationBuilder);
          mutableModel.MergeObjects(this.Model.GetObject(interfaceObject).ToMutable(), interfaceBuilder);
          this.Model = mutableModel.ToImmutable();
      }
      public void RemoveOperationFromInterface(ImmutableObject operationObject)
      {
          this.RemoveObject(operationObject);
      }
              //END INTERFACE realted SET methods
      //END INTERFACE related methods
    */
      public ImmutableObject CreateParameter(string parameterName)
      {
          var mutableModel = this.model.Model.ToMutable();
          var parameterBuilder = this.factory.Parameter();
          parameterBuilder.MName = parameterName;
          mutableModel.ResolveObject(parameterBuilder.MId);
          this.model.Model = mutableModel.ToImmutable();
          return parameterBuilder.ToImmutable();
      }
       /*

      public void SetParameter(ImmutableObject originParameterObject,ImmutableObject targetParameterObject)
      {
          var mutableModel = this.Model.ToMutable();
          var targetParameterBuilder = (ParameterBuilder)targetParameterObject.ToMutable();
          mutableModel.MergeObjects(this.Model.GetObject(originParameterObject).ToMutable(), targetParameterBuilder);

          this.Model = mutableModel.ToImmutable();
      }
       */
      public void SetParameterToOperation(ImmutableObject operationObject,ImmutableObject parameterObject, ParameterDirectionKind kind)
      {
          var mutableModel = this.model.Model.ToMutable();

          var operationBuilder = (OperationBuilder)operationObject.ToMutable();
          var parameterBuilder = (ParameterBuilder)parameterObject.ToMutable();

          parameterBuilder.SetDirectionLazy(parameterBuilder1 => kind);
          parameterBuilder.SetOperationLazy(parameterBuilder1 => operationBuilder);
          mutableModel.MergeObjects(this.model.Model.GetObject(operationObject).ToMutable(), operationBuilder);
          mutableModel.MergeObjects(this.model.Model.GetObject(parameterObject).ToMutable(), parameterBuilder);
          this.model.Model = mutableModel.ToImmutable();
          this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
        }
    

      public ImmutableObject CreateEnumLiteral(string literalName)
      {
          var mutableModel = this.model.Model.ToMutable();
          var enumerationLiteralBuilder = this.factory.EnumerationLiteral();
          enumerationLiteralBuilder.MName = literalName;
          mutableModel.ResolveObject(enumerationLiteralBuilder.MId);
          this.model.Model = mutableModel.ToImmutable();
          return enumerationLiteralBuilder.ToImmutable();
      }
      public void SetEnumerationLiteralToEnumeration(ImmutableObject enumerationObject,ImmutableObject enumerationLiteralObject)
      {
          var mutableModel = this.model.Model.ToMutable();

          var enumerationBuilder = (EnumerationBuilder)enumerationObject.ToMutable();
          var enumerationLiteralBuilder = (EnumerationLiteralBuilder)enumerationLiteralObject.ToMutable();

          enumerationLiteralBuilder.SetEnumerationLazy(elb => enumerationBuilder);

          mutableModel.MergeObjects(this.model.Model.GetObject(enumerationObject).ToMutable(), enumerationBuilder);
          mutableModel.MergeObjects(this.model.Model.GetObject(enumerationLiteralObject).ToMutable(), enumerationLiteralBuilder);

          this.model.Model = mutableModel.ToImmutable();
          this.graphLayoutModel.ReloadGraphLayoutModel(this.model);

        }

      //REALIZATION -->> Classes can implement/realize interfaces
     
        public ImmutableObject CreateInterfaceRealization()
      {
          var mutableModel = this.model.Model.ToMutable();
          var realizationBuilder = this.factory.InterfaceRealization();
          mutableModel.ResolveObject(realizationBuilder.MId);
          this.model.Model = mutableModel.ToImmutable();
            return this.model.Model.GetObject(realizationBuilder);
        }
     
      public void SetInterfaceRealization(ImmutableObject interfaceObject,ImmutableObject implementingClassifierObject, ImmutableObject interfaceRealizationObject)
      {
          var mutableModel = this.model.Model.ToMutable();

          var interfaceBuilder = (InterfaceBuilder)interfaceObject.ToMutable();
          var classifierBuilder = (BehavioredClassifierBuilder)implementingClassifierObject.ToMutable();
          var interfaceRealizationBuilder = (InterfaceRealizationBuilder)interfaceRealizationObject.ToMutable();

          interfaceRealizationBuilder.SetContractLazy(interfaceRealizationBuilder1 => interfaceBuilder);
          interfaceRealizationBuilder.SetImplementingClassifierLazy(interfaceRealizationBuilder1 => classifierBuilder);

          mutableModel.MergeObjects(this.model.Model.GetObject(interfaceObject).ToMutable(), interfaceBuilder);
          mutableModel.MergeObjects(this.model.Model.GetObject(implementingClassifierObject).ToMutable(), classifierBuilder);
          mutableModel.MergeObjects(this.model.Model.GetObject(interfaceRealizationObject).ToMutable(), interfaceRealizationBuilder);
          this.model.Model = mutableModel.ToImmutable();
          this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
        }

        

      public ImmutableObject CreateGeneralization()
      {
          var mutableModel = this.model.Model.ToMutable();
          var generalizationBuilder = this.factory.Generalization();
          mutableModel.ResolveObject(generalizationBuilder.MId);
          this.model.Model = mutableModel.ToImmutable();
          return generalizationBuilder.ToImmutable();
      }
      public void SetGeneralization(ImmutableObject generalObject,ImmutableObject specificObject,ImmutableObject generalizationObject)
      {
          var mutableModel = this.model.Model.ToMutable();

          var generalClassifierBuilder = (ClassifierBuilder)generalObject.ToMutable();
          var specificClassifierBuilder = (ClassifierBuilder)specificObject.ToMutable();
          var generalizationBuilder = (GeneralizationBuilder)generalizationObject.ToMutable();

          generalizationBuilder.SetGeneralLazy(generalizationBuilder1 => generalClassifierBuilder);
          generalizationBuilder.SetSpecificLazy(generalizationBuilder1 => specificClassifierBuilder);
          mutableModel.MergeObjects(this.model.Model.GetObject(generalObject).ToMutable(), generalClassifierBuilder);
          mutableModel.MergeObjects(this.model.Model.GetObject(specificObject).ToMutable(), specificClassifierBuilder);
          mutableModel.MergeObjects(this.model.Model.GetObject(generalizationObject).ToMutable(), generalizationBuilder);
          this.model.Model = mutableModel.ToImmutable();
          this.graphLayoutModel.ReloadGraphLayoutModel(this.model);
        }

        /*
      //Dependency
      public ImmutableObject CreateDependency()
      {
          var mutableModel = this.Model.ToMutable();
          var dependencyBuilder  = this.factory.Dependency();
          mutableModel.ResolveObject(dependencyBuilder.MId);

          this.Model = mutableModel.ToImmutable();
          return dependencyBuilder.ToImmutable();
      }

      public void SetDependency(ImmutableObject dependecyObject, ImmutableObject clientObject, ImmutableObject supplierObject)
      {

      }

      public ImmutableObject CreateAssociation()
      {
          var mutableModel = this.Model.ToMutable();
          var associationBuilder = this.factory.Association();

          mutableModel.ResolveObject(associationBuilder.MId);
          this.Model = mutableModel.ToImmutable();
          return associationBuilder.ToImmutable();
      }
      public void SetAssociation(ImmutableObject associoationObject,ImmutableObject associationEndObject,ImmutableObject propertyObject)
      {
          var mutableModel = this.Model.ToMutable();
          var associationBuilder = (AssociationBuilder)associoationObject.ToMutable();

      }
      public void SetAggregation(ImmutableObject aggreationObject, ImmutableObject aggregationEndObject, ImmutableObject propertyObject)
      {
          var mutableModel = this.Model.ToMutable();
          var associationBuilder = (AssociationBuilder)aggreationObject.ToMutable();
          var mutableAggregationEnd = aggregationEndObject.ToMutable();
          var aggregationPropertyBuilder = (PropertyBuilder)propertyObject.ToMutable();
          aggregationPropertyBuilder.SetAggregationLazy(associationPropertyBuilder => AggregationKind.Shared);
      }
      public void SetComposition(ImmutableObject aggreationObject, ImmutableObject aggregationEndObject, ImmutableObject propertyObject)
      {

      }
      public ImmutableObject CreatePackage(string packaName)
      {
          var mutableModel = this.Model.ToMutable();
          var packageBuilder = this.factory.Package();
          packageBuilder.MName = packaName;
          mutableModel.ResolveObject(packageBuilder.MId);
          this.Model = mutableModel.ToImmutable();
          return packageBuilder.ToImmutable();
      }
      private void RemoveObject(ImmutableObject obj)
      {

          var mutableModel = this.Model.ToMutable();
          if (mutableModel.RemoveObject(obj.ToMutable()))
          {
              this.Model = mutableModel.ToImmutable();
          }

      }


      protected void AddClasses(IEnumerable<Class> classes)
      {
          foreach (var cls in classes)
          {
              if (cls.MMetaClass.Name == "Class" && cls.MParent.MName == "Logical View")
              {
                  this.GraphLayout.AddNode(cls);
              }
          }
      }

      */

    }


    public class DiagramController
    {

        private Connector _connector;

        public Connector Connector
        {
            get { return this._connector; }
        }

        public DiagramController(Connector connector)
        {
            this._connector = connector;
        }

        public bool Open()
        {
            if (this._connector.Equals(null)) return false;

            this._connector.ConnectUMLModelAndGraphLayout();
            return true;
        }

    }
}
