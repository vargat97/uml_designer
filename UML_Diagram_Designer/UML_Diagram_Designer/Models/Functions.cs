using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using Stylet;
using System;
using System.Collections.Generic;
using System.Text;

namespace UML_Diagram_Designer.Models
{
    public class Functions
    {

    

    private ImmutableModel _immutableModel;
        public Functions(ImmutableModel immutableModel )
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
            this.RemoveOperationObjectAndItsChildren(immutableObject);
            return this._immutableModel;
        }
        /// <summary>
        /// Removes the given Class object, and its all children from the model</summary>
        /// <param name="classObject">Class object, what we would like to remove from the model</param>
        /// <returns>Return a Model, which does not contain the given object</returns>
        public ImmutableModel RemoveClassObjectFromModel(Class classObject)
        {
            
            foreach(var child in classObject.MChildren)
            {
                this.RemoveClassifierChilds(child);
            }
            this.RemoveGeneralizationDependsOnClass(classObject);
            this.RemoveDependencyDependsOnClassifier(classObject);
            this.RemoveInterfaceRealizationDependsOnClass(classObject);
           this.RemoveObject(classObject);
            return this._immutableModel;
        }

        public ImmutableModel RemoveInterfaceFromModel(Interface interfaceObject)
        {
            foreach (var child in interfaceObject.MChildren)
            {
                this.RemoveClassifierChilds(child);
            }
            this.RemoveInterfaceRealizationDependsOnInterface(interfaceObject);
            this.RemoveDependencyDependsOnClassifier(interfaceObject);
            this.RemoveAssociationEndParamaterDependsOnClassifier(interfaceObject);
            this.RemoveObject(interfaceObject);
            return this._immutableModel;
        }


        private void RemoveClassifierChilds(ImmutableObject child)
        {
            if(child.MMetaClass.MName == "Operation")
            {
                this.RemoveOperationObjectAndItsChildren((Operation)child);
            }
            else if(child.MMetaClass.MName == "Property")
            {
                this.RemovePropertyFromClass((Property)child);
            }
        }
        /// <summary>
        /// Removes all the parameters of the operations
        /// Removes the operation from the model
        /// </summary>
        /// <param name="operation">The operation what we would like to remove</param>
        /// <returns>No return value</returns>
        private void RemoveOperationObjectAndItsChildren(Operation operation)
        {
            foreach (Parameter par in operation.MChildren)
            {
                this.RemoveObject(par);
            }

            this.RemoveObject(operation);
        }
        
        /// <summary>
        /// Removes the property from the model
        /// If property is member of an association, then search the second end of the association, removes it also, and removes the association too
        /// </summary>
        /// <param name="property">Property, what we would like to remove</param>
        /// <returns>No return value</returns>
        private void RemovePropertyFromClass(Property property)
        {
            var association = property.Association;
            if (association == null)
            {
                this.RemoveObject(property);
                return;
            }

            var members = association.Member;
            foreach(var member in members)
            {
                this.RemoveObject(member);
            }
            this.RemoveObject(association);
        }
        /// <summary>
        /// Removes all the generalization depends on the parameter classObject
        /// </summary>
        /// <param name="classObject">The generalizaton GENERAL side</param>
        private void RemoveGeneralizationDependsOnClass(ImmutableObject classObject)
        {
            foreach(var modelObject in this._immutableModel.Objects)
            {
                if(modelObject.MMetaClass.Name == "Generalization")
                {
                    var generalization = (GeneralizationBuilder)modelObject.ToMutable();
                    if(generalization.General.MName == classObject.MName || generalization.Specific.MName == classObject.MName)
                    {
                        generalization.SetGeneralLazy(cb => null);
                        this.RemoveObject(generalization.ToImmutable());
                    }
                }
            }
        }
        private void RemoveInterfaceRealizationDependsOnClass(Class classObject)
        {
            if(classObject.InterfaceRealization != null)
            {
                foreach(var interfaceRealization in classObject.InterfaceRealization)
                {
                    var ifrbuilder = interfaceRealization.ToMutable();
                    ifrbuilder.SetOwnerLazy(eb => null);
                    ifrbuilder.SetImplementingClassifierLazy(bcb => null);
                    this.RemoveObject(interfaceRealization);
                }
            }
        }
        private void RemoveAssociationEndParamaterDependsOnClassifier(Classifier classifierObject)
        {
            foreach (var modelObject in this._immutableModel.Objects)
            {
                if (modelObject.MMetaClass.Name == "Association")
                {
                    var association = (AssociationBuilder)modelObject.ToMutable();
                    var isRemovable = false;
                    foreach(var member in association.Member)
                    {
                        if (member.MType.MName == classifierObject.MName) isRemovable = true;
                    }

                    if (isRemovable)
                    {
                        foreach (var member in association.Member) this.RemoveObject(member.ToImmutable());
                        this.RemoveObject(association.ToImmutable());
                    }
                }
            }
        }
        private void RemoveInterfaceRealizationDependsOnInterface(Interface interfaceObject)
        {
            foreach (var modelObject in this._immutableModel.Objects)
            {
                if (modelObject.MMetaClass.Name == "InterfaceRealization")
                {
                    var interfacegb = (InterfaceRealizationBuilder)modelObject.ToMutable();
                    bool isRemovable = false;
                    foreach(var supplier in interfacegb.Supplier)
                    {
                        if(supplier.MName == interfaceObject.MName)
                        {

                            isRemovable = true;
                        }
                    }

                    if (isRemovable)
                    {
                        interfacegb.SetOwnerLazy(eb => null);
                        interfacegb.SetImplementingClassifierLazy(bcb => null);
                        this.RemoveObject(interfacegb.ToImmutable());
                    }
                }
            }
        }
        private void RemoveDependencyDependsOnClassifier(ImmutableObject classObject)
        {
            foreach (var modelObject in this._immutableModel.Objects)
            {
                if (modelObject.MMetaClass.Name == "Dependency")
                {
                    var dependency = (DependencyBuilder)modelObject.ToMutable();
                    bool isRemovable = false;
                    foreach(var client in dependency.Client)
                    {
                        if (client.MName == classObject.MName)
                            isRemovable = true;                                             
                    }
                    foreach(var supplier in dependency.Supplier)
                    {
                        if (supplier.MName == classObject.MName)
                            isRemovable = true;
                    }

                    if (isRemovable)
                    {
                        dependency.SetOwnerLazy(eb => null);
                        this.RemoveObject(dependency.ToImmutable());
                    }
                       
                }
            }
        }
        /// <summary>
        /// Removes the immutableObject from the Model
        /// </summary>
        /// <param name="immutableObject">Object, what we would like to remove from the model</param>
        /// <returns>No return value</returns>
        private void RemoveObject(ImmutableObject immutableObject)
        {
         
            var mutableModel = this._immutableModel.ToMutable();

            //If Model does not contain the object, then return
            if (mutableModel.GetObject(immutableObject) == null) return;

            //If Model can remove the object from it side, then set the new model as _immutableModel, which no doesnt contains the object
            if (mutableModel.RemoveObject(immutableObject.ToMutable()) ){
                this._immutableModel = mutableModel.ToImmutable();
                
            }
        }
        /// <summary>
        /// Removes the interfacerealization from the model. Removes the connection from the two side of the realiztion, then remove itself
        /// Removes also the realized interface's attributes and operations from the owner's classifier
        /// </summary>
        /// <param name="immutableObject">Interfacerealization, what we would like to remove</param>
        /// <returns></returns>
        public ImmutableModel RemveInterfaceRealizationFromModel(InterfaceRealization immutableObject)
        {
            var mutableObejct = immutableObject.ToMutable();
            mutableObejct.SetOwnerLazy(eb => null);
            mutableObejct.SetContractLazy(ib => null);
            this.RemoveObject(mutableObejct.ToImmutable());
            return this._immutableModel;
        }
        public ImmutableModel RemoveGeneralizationFromModel(ImmutableObject immutableObject)
        {
            this.RemoveObject(immutableObject);
            return this._immutableModel;
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
