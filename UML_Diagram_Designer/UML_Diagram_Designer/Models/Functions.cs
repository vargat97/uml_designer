using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public class Functions
    {
        private ImmutableModel _immutableModel;
        public Functions(ImmutableModel immutableModel)
        {
            this._immutableModel = immutableModel;
        }
        public ImmutableModel CreateRelationship(ImmutableObject source, ImmutableObject dest, ImmutableObject relationship)
        {
            var relationshipType = relationship.GetType();
            if(relationshipType.Name == "AssociationImpl")
            {
                var my_relationship = (Association)relationship;
                this.CreateAssociation(source, dest, my_relationship);
            }
            else if(relationshipType.Name == "InterfaceRealizationImpl")
            {
                var my_relationship = (InterfaceRealization)relationship;
                this._immutableModel = this.CreateInterfaceRealization(source, dest, my_relationship);
            }
            else if(relationshipType.Name == "DependencyImpl")
            {
                var my_relationship = (Dependency)relationship;
                this._immutableModel = this.CreateDependency(source, dest, my_relationship);
            }
            else if(relationshipType.Name == "GeneralizationImpl")
            {
                var my_relationship = (Generalization)relationship;
                this._immutableModel = this.CreateGeneralization(dest, source, my_relationship);
            }

            return this._immutableModel;
        }
        public ImmutableModel RemovePropertyObject(Property immutableObject)
        {
            if(immutableObject.Association != null)
            {
                var association = this._immutableModel.GetObject(immutableObject.Association);
               this.RemoveObject(association);
            }
            if(immutableObject.Aggregation != AggregationKind.None)
            {

            }
            this.RemoveObject(immutableObject);
            return this._immutableModel;
        }
        public ImmutableModel RemoveOperationObject(Operation immutableObject)
        {
            foreach(Parameter par in immutableObject.MChildren)
            {
                this.RemoveObject(par);
            }

            this.RemoveObject(immutableObject);
            return this._immutableModel;
        }

        private void RemoveObject(ImmutableObject immutableObject)
        {
            var mutableModel = this._immutableModel.ToMutable();

            if (mutableModel.GetObject(immutableObject) == null) return;

            if (mutableModel.RemoveObject(immutableObject.ToMutable()) ){
                this._immutableModel = mutableModel.ToImmutable();
                
            }
        }

        public ImmutableModel ModifyObjectName(ImmutableObject obj, string newName)
        {
            if (this._immutableModel.ContainsObject(obj))
            {

                var mutableModel = this._immutableModel.ToMutable();
                var mutableObject = mutableModel.GetObject(obj.ToMutable());
                var namedElement = (MetaDslx.Languages.Uml.Model.NamedElement)obj;

                var namedElementBuilder = namedElement.ToMutable();
                namedElementBuilder.SetNameLazy(n => newName);
                mutableModel.MergeObjects(mutableModel.GetObject(mutableObject), namedElementBuilder);
                return mutableModel.ToImmutable();
            }
            return this._immutableModel;
        }
        public ImmutableModel CreateEnumLiteral(ImmutableObject enumObject, string name)
        {
            var enumLiteral = (EnumerationLiteral)this._createEnumLiteral(name);
            var mutableModel = this._immutableModel.ToMutable();

            var enumLiteralBuilder = enumLiteral.ToMutable();
            mutableModel.ResolveObject(enumLiteralBuilder.MId);

            var enumBuilder = (EnumerationBuilder)enumObject.ToMutable();

            enumLiteralBuilder.SetEnumerationLazy(elb => enumBuilder);

            mutableModel.MergeObjects(mutableModel.GetObject((Enumeration)enumObject), enumBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(enumLiteral), enumLiteralBuilder);
            return mutableModel.ToImmutable();
        }
        private ImmutableObject _createEnumLiteral(string name)
        {
                var mutableModel = this._immutableModel.ToMutable();
                var factory = new UmlFactory(mutableModel);
                var enumerationLiteralBuilder = factory.EnumerationLiteral();
                enumerationLiteralBuilder.MName = name;
               
                return enumerationLiteralBuilder.ToImmutable();
            
        }
        public ImmutableObject CreateAssociation()
        {
            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var associoationBuilder = factory.Association();
            return associoationBuilder.ToImmutable();
        }
        public ImmutableObject CreateGeneralization()
        {
            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var generalizationBuilder = factory.Generalization();
            return generalizationBuilder.ToImmutable();
        }
        public ImmutableObject CreateInterfaceRealization()
        {
            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var interfaceRealizationBuilder = factory.InterfaceRealization();
            return interfaceRealizationBuilder.ToImmutable();
        }
        public ImmutableObject CreateDependency()
        {
            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var dependencyBuilder = factory.Dependency();
            return dependencyBuilder.ToImmutable();
        }
        public ImmutableModel ModifyObjectVisivility(ImmutableObject obj, VisibilityKind kind)
        {
            if (this._immutableModel.ContainsObject(obj))
            {
                var mutableModel = this._immutableModel.ToMutable();
                var mutableObject = mutableModel.GetObject(obj.ToMutable());
                var namedElemen = (MetaDslx.Languages.Uml.Model.NamedElement)obj;

                var namedElementBuilder = namedElemen.ToMutable();
                namedElementBuilder.SetVisibilityLazy(namedElementBuilder1 => kind);
                mutableModel.MergeObjects(mutableObject, namedElementBuilder);
                return mutableModel.ToImmutable();
            }

            return this._immutableModel;

        }
        
        public ImmutableModel SetObjectToAbstract(ImmutableObject obj)
        {
            var mutableModel = this._immutableModel.ToMutable();
            var classifierBuilder = (ClassifierBuilder)obj.ToMutable();
            var isAbstract = classifierBuilder.IsAbstract;

            classifierBuilder.SetIsAbstractLazy(classifierbuilderabstract => !isAbstract);
            mutableModel.MergeObjects(mutableModel.GetObject(obj), classifierBuilder);

            return mutableModel.ToImmutable();
        }
        public ImmutableModel AddNewParameterToOperation(string parameterName,ImmutableObject parameterType,ImmutableObject operationObject)
        {
            var parameter = this.CreateParameter(parameterName);
            this.SetObjectType(parameter, parameterType);
            var parameterDirection = ParameterDirectionKind.In;
            this.setParameterToOperation(operationObject, parameter, parameterDirection);

            return this._immutableModel;
        }
       
        private void CreateAssociation(ImmutableObject source, ImmutableObject dest, Association relationship)
        {
            var mutableModel = this._immutableModel.ToMutable();

        }

        private ImmutableModel CreateInterfaceRealization(ImmutableObject client, ImmutableObject supplier, InterfaceRealization relationship)
        {
            var mutableModel = this._immutableModel.ToMutable();
            var interfaceRealizationBuilder = relationship.ToMutable();
            mutableModel.ResolveObject(interfaceRealizationBuilder.MId);
            
            var clientBuilder = (BehavioredClassifierBuilder)client.ToMutable();
            var interfaceBuilder = (InterfaceBuilder)supplier.ToMutable();
            interfaceRealizationBuilder.SetContractLazy(interfaceRealizationBuilder1 => interfaceBuilder);
            interfaceRealizationBuilder.SetImplementingClassifierLazy(interfaceRealizationBuilder1 => clientBuilder);

            mutableModel.MergeObjects(mutableModel.GetObject(supplier), interfaceBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(client), clientBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(interfaceRealizationBuilder), interfaceRealizationBuilder);

            return mutableModel.ToImmutable();
        }

        private ImmutableModel CreateGeneralization(ImmutableObject parent, ImmutableObject child, Generalization relationship)
        {
            var mutableModel = this._immutableModel.ToMutable();
            var generalizationBuilder = relationship.ToMutable();
            mutableModel.ResolveObject(generalizationBuilder.MId);

            var generalClassifierBuilder = (ClassifierBuilder)parent.ToMutable();
            var specificClassifierBuilder = (ClassifierBuilder)child.ToMutable();

            generalizationBuilder.SetGeneralLazy(generalizationBuilder1 => generalClassifierBuilder);
            generalizationBuilder.SetSpecificLazy(generalizationBuilder1 => specificClassifierBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(parent), generalClassifierBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(child), specificClassifierBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(generalizationBuilder), generalizationBuilder);
            return mutableModel.ToImmutable();

        }

        private ImmutableModel CreateDependency(ImmutableObject client, ImmutableObject supplier, Dependency relationship)
        {
            var mutableModel = this._immutableModel.ToMutable();
            var dependencyBuilder = relationship.ToMutable();
            mutableModel.ResolveObject(dependencyBuilder.MId);

              //TODO

            return mutableModel.ToImmutable();
        }
        private void setParameterToOperation(ImmutableObject operationObject, ImmutableObject parameterObject, ParameterDirectionKind kind)
      {
            var mutableModel = this._immutableModel.ToMutable();

            var operationBuilder = (OperationBuilder)operationObject.ToMutable();
            var parameterBuilder = (ParameterBuilder)parameterObject.ToMutable();

            parameterBuilder.SetDirectionLazy(parameterBuilder1 => kind);
            parameterBuilder.SetOperationLazy(parameterBuilder1 => operationBuilder);
            // mergeobject(mutablemodel)
            mutableModel.MergeObjects(this._immutableModel.GetObject(operationObject).ToMutable(), operationBuilder);
            mutableModel.MergeObjects(this._immutableModel.GetObject(parameterObject).ToMutable(), parameterBuilder);
            this._immutableModel = mutableModel.ToImmutable();
        }
        private ImmutableObject CreateParameter(string parameterName)
        {
            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var parameterBuilder = factory.Parameter();
            parameterBuilder.MName = parameterName;
            mutableModel.ResolveObject(parameterBuilder.MId);
            this._immutableModel = mutableModel.ToImmutable();
            return parameterBuilder.ToImmutable();
        }
        private void SetObjectType(ImmutableObject immutableObject, ImmutableObject typeObject)
        {
            var mutableModel = this._immutableModel.ToMutable();
            var typedElementBuilder = (TypedElementBuilder)immutableObject.ToMutable();
            var typeBuilder = (TypeBuilder)typeObject.ToMutable();

            typedElementBuilder.SetTypeLazy(t => typeBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(immutableObject), typedElementBuilder);

            this._immutableModel = mutableModel.ToImmutable();
        }
    }
}
