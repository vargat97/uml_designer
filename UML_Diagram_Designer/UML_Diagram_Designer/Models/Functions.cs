using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using RandomNameGeneratorLibrary;
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
        /// <summary>
        /// Removes the property from the model. If the property is member of an association, then removes the association too
        /// </summary>
        /// <param name="immutableObject">Property, what we would like to remove</param>
        /// <returns>Returns an ImmutableModel, which does not contain the property</returns>
        public ImmutableModel RemovePropertyObject(Property immutableObject)
        {
            if(immutableObject.Association != null)
            {
                var association = this._immutableModel.GetObject(immutableObject.Association);
               this.RemoveObject(association);
            }
            if(immutableObject.Aggregation != AggregationKind.None)
            {
                //TODO
            }
            this.RemoveObject(immutableObject);
            return this._immutableModel;
        }
        /// <summary>
        /// Removes the operation, and its children from the model
        /// </summary>
        /// <param name="immutableObject">Operation, what we would like to remove</param>
        /// <returns>Return an ImmutableModel, which doesnot contain the operation</returns>
        public ImmutableModel RemoveOperationObject(Operation immutableObject)
        {
            this.RemoveOperationObjectAndItsChildren(immutableObject);
            return this._immutableModel;
        }

        /// <summary>
        /// Removes the selected Parameter from the model
        /// </summary>
        /// <param name="parameter">Parameter, what we would like to remove from model</param>
        /// <returns>An ImmutableModel, which does not contain the selected parameter</returns>
        public ImmutableModel RemoveParameterFromModel(Parameter parameter)
        {
            this.RemoveObject(parameter);
            return this._immutableModel;
        }

        /// <summary>
        /// Creates an attribute for the given class and set its name as random
        /// </summary>
        /// <param name="classObject">Class object, where we would like to add a new attribute</param>
        /// <returns></returns>
        public ImmutableModel CreateAttributeToClass(Class classObject)
        {
            //Create the property/attribute
            var propertyBuilder = this.CreateProperty().ToMutable();
            var mutableModel = this._immutableModel.ToMutable();

            //Resolve the property
            mutableModel.ResolveObject(propertyBuilder.MId);

            var propertyObject = propertyBuilder.ToImmutable();
            var classBuilder = classObject.ToMutable();
            propertyBuilder.SetClassLazy(propertyBuilder1 => classBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(propertyObject), propertyBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(classObject), classBuilder);

            this._immutableModel = mutableModel.ToImmutable();
            return this._immutableModel;
        }
        /// <summary>
        /// Creates an operation to the given class with a random name
        /// </summary>
        /// <param name="classObject">The class object, where we would like to create an operation</param>
        /// <returns>A new immutableModel, which noe contains the new operation</returns>
        public ImmutableModel CreateOperationToClass(Class classObject)
        {
            var operationBuilder = this.CreateOperation().ToMutable();
            var mutableModel = this._immutableModel.ToMutable();

            mutableModel.ResolveObject(operationBuilder.MId);

            var operationObject = operationBuilder.ToImmutable();
            var classBuilder = classObject.ToMutable();
            operationBuilder.SetClassLazy(cb => classBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(operationObject), operationBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(classObject), classBuilder);

            this._immutableModel = mutableModel.ToImmutable();
            return this._immutableModel;
        }
        /// <summary>
        /// Creates an attribute for the given interface and set its name as random
        /// </summary>
        /// <param name="interfaceObject">Interface object, where we would like to add a new attribute</param>
        /// <returns></returns>
        public ImmutableModel CreateAttributeToInterface(Interface interfaceObject)
        {
            var propertyBuilder = this.CreateProperty().ToMutable();
            var mutableModel = this._immutableModel.ToMutable();

            mutableModel.ResolveObject(propertyBuilder.MId);

            var propertyObject = propertyBuilder.ToImmutable();
            var interfaceBuilder = interfaceObject.ToMutable();
            propertyBuilder.SetInterfaceLazy(cb => interfaceBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(propertyObject), propertyBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(interfaceObject), interfaceBuilder);

            this._immutableModel = mutableModel.ToImmutable();
            return this._immutableModel;
        }
        /// <summary>
        /// Creates an operation to the given interface with a random name
        /// </summary>
        /// <param name="interfaceObject">The interface object, where we would like to create an operation</param>
        /// <returns>A new immutableModel, which noe contains the new operation</returns>
        public ImmutableModel CreateOperationToInterface(Interface interfaceObject)
        {
            var operationBuilder = this.CreateOperation().ToMutable();
            var mutableModel = this._immutableModel.ToMutable();

            mutableModel.ResolveObject(operationBuilder.MId);

            var operationObject = operationBuilder.ToImmutable();
            var interfaceBuilder = interfaceObject.ToMutable();

            operationBuilder.SetInterfaceLazy(ib => interfaceBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(operationObject), operationBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(interfaceObject), interfaceBuilder);

            this._immutableModel = mutableModel.ToImmutable();
            return this._immutableModel;
        }
        /// <summary>
        /// Removes the given Class object, and its all children from the model
        /// Removes the appropriate interfacerealizations, dependencies, and associations from the model
        /// </summary>
        /// <param name="classObject">Class object, what we would like to remove from the model</param>
        /// <returns>Return a Model, which does not contain the given object</returns>
        public ImmutableModel RemoveClassObjectFromModel(Class classObject)
        {
            
            foreach(var child in classObject.MChildren)
            {
                this.RemoveClassifierChilds(child);
            }
            this.RemoveGeneralizationDependsOnClass(classObject);
            this.RemoveDependencyDependsOnImmutableObject(classObject);
            this.RemoveInterfaceRealizationDependsOnClass(classObject);
            this.RemoveObject(classObject);
            return this._immutableModel;
        }

        /// <summary>
        /// Removes the given Interface object from the model, and its all children from the model
        /// Removes the all relevan interfacerealizations, dependencies, association from the model too
        /// </summary>
        /// <param name="interfaceObject"></param>
        /// <returns></returns>
        public ImmutableModel RemoveInterfaceFromModel(Interface interfaceObject)
        {
            foreach (var child in interfaceObject.MChildren)
            {
                this.RemoveClassifierChilds(child);
            }
            this.RemoveInterfaceRealizationDependsOnInterface(interfaceObject);
            this.RemoveDependencyDependsOnImmutableObject(interfaceObject);
            this.RemoveAssociationEndParamaterDependsOnClassifier(interfaceObject);
            this.RemoveObject(interfaceObject);
            return this._immutableModel;
        }

        /// <summary>
        /// Removes the class child from the model
        /// If operation, than calls RemoveOperationObjectAndItsChildren
        /// else calls RemovePropertyFromClass
        /// </summary>
        /// <param name="child"></param>
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
        
        /// <summary>
        /// Removes all the interfacerealization what tha class object realized previously
        /// </summary>
        /// <param name="classObject">A class object, what we would like to remove, and for that, we have to remove all the appropriate realized interfaces</param>
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
        
        /// <summary>
        /// Removes the association and all members of the association, if one of the member is the parameter classifierobject
        /// </summary>
        /// <param name="classifierObject">For the classifierObject remove, have to remove all the appropriate association and parameter</param>
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
        
        /// <summary>
        /// Removes all the interfacerealization what belongs to that given interface
        /// </summary>
        /// <param name="interfaceObject">Interface object, what we would like to remove</param>
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
        /// <summary>
        /// Removes the dependencies which are depends on the classifier object
        /// For the classifier remove, we had to remove the appropriate dependencies too
        /// </summary>
        /// <param name="classObject">A classifier object, from what the dependency is depends on</param>
        private void RemoveDependencyDependsOnImmutableObject(ImmutableObject classObject)
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
        /// Creates a property object with a random name
        /// </summary>
        /// <returns>Returns with the created property object</returns>
        private Property CreateProperty()
        {
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();

            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var propertyBuilder = factory.Property();
            propertyBuilder.MName = name;

            return propertyBuilder.ToImmutable();
        }
        /// <summary>
        /// Creates an operation with a random name
        /// </summary>
        /// <returns>Returns with the created operation object</returns>
        private Operation CreateOperation()
        {
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();

            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var operationBuilder = factory.Operation();
            operationBuilder.MName = name;

            return operationBuilder.ToImmutable();
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
        
        /// <summary>
        /// Removes the given enum and its child from the model
        /// </summary>
        /// <param name="enumObject"></param>
        /// <returns></returns>
        public ImmutableModel RemoveEnumFromModel(Enumeration enumObject)
        {
            foreach(var child in enumObject.MChildren)
            {
                this.RemoveObject(child);
            }
            this.RemoveDependencyDependsOnImmutableObject(enumObject);
            this.RemoveObject(enumObject);
            return this._immutableModel;
        }

        /// <summary>
        /// Removes the selected generalization from the model
        /// </summary>
        /// <param name="immutableObject">Generalization, what we would like to remove</param>
        /// <returns>An ImmutableModel which does not contain the selected generalization</returns>
        public ImmutableModel RemoveGeneralizationFromModel(Generalization immutableObject)
        {
            var generalizationBuilder = immutableObject.ToMutable();
            generalizationBuilder.SetGeneralLazy(cb => null);
            generalizationBuilder.SetSpecificLazy(cb => null);
            this.RemoveObject(immutableObject);
            return this._immutableModel;
        }
        
        /// <summary>
        /// Removes the selected dependency from the model
        /// </summary>
        /// <param name="immutableObject">Dependenc object what we would like to remove</param>
        /// <returns>An immutableModel which does not contain the selected generalization</returns>
        public ImmutableModel RemoveDependencyFromModel(Dependency immutableObject)
        {
            var dependencyBuilder = immutableObject.ToMutable();
            dependencyBuilder.SetOwnerLazy(eb => null);
            this.RemoveObject(dependencyBuilder.ToImmutable());
            return this._immutableModel;
        }

        /// <summary>
        /// Removes enumerationliteral from the model
        /// </summary>
        /// <param name="literal"></param>
        /// <returns>a ImmutableModel which does not contain the literal</returns>
        public ImmutableModel RemoveEnumerationLiteralFromModel(EnumerationLiteral literal)
        {
            this.RemoveObject(literal);
            return this._immutableModel;
        }

        public ImmutableModel RemoveAssociationFromModel(Association association)
        {
            foreach(var member in association.Member)
            {
                this.RemoveObject(member);
            }
            this.RemoveObject(association);
            return this._immutableModel;
        }


        /// <summary>
        /// Modify the selected object's name. 
        /// </summary>
        /// <param name="obj">Selected namedElement, which name we want to modify</param>
        /// <param name="newName">New name</param>
        /// <returns>An ImmutableModel, which contains now the namedelement whit the new name</returns>
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

        public ImmutableModel ModifyObjectType(ImmutableObject immutableObject, ImmutableObject type)
        {
            if (!this._immutableModel.ContainsObject(immutableObject) || !this._immutableModel.ContainsObject(type)) return this._immutableModel;
            var mutableModel = this._immutableModel.ToMutable();
            var typedElementBuilder = (TypedElementBuilder)immutableObject.ToMutable();
            var typeBuilder = (TypeBuilder)type.ToMutable();

            typedElementBuilder.SetTypeLazy(tb => typeBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(immutableObject), typedElementBuilder);
            this._immutableModel = mutableModel.ToImmutable();

            return this._immutableModel;
        }

        public ImmutableModel CreateClass()
        {
            var classObject = this.CreateClassObject();
            var mutableModel = this._immutableModel.ToMutable();

            mutableModel.ResolveObject(classObject.MId);
            mutableModel.MergeObjects(mutableModel.GetObject(classObject), classObject.ToMutable());
            this._immutableModel = mutableModel.ToImmutable();

            return this._immutableModel;
        }

        private ImmutableObject CreateClassObject()
        {
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();

            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var classBuilder = factory.Class();
            classBuilder.MName = name;

            return classBuilder.ToImmutable();
        }

        /// <summary>
        /// Creates a new EnumearationLiteral for the enumObject with a random name
        /// </summary>
        /// <param name="enumObject">The enumeration object, where we would like to create a new enumerationliteral object</param>
        /// <returns>ImmutableModel, which contains the created enumerationLiteral</returns>
        public ImmutableModel CreateEnumLiteralToEnumeration(ImmutableObject enumObject)
        {

            var enumLiteral = (EnumerationLiteral)this.CreateEnumLiteral();
            var mutableModel = this._immutableModel.ToMutable();

            var enumLiteralBuilder = enumLiteral.ToMutable();
            mutableModel.ResolveObject(enumLiteralBuilder.MId);

            var enumBuilder = (EnumerationBuilder)enumObject.ToMutable();
            enumLiteralBuilder.SetEnumerationLazy(elb => enumBuilder);

            mutableModel.MergeObjects(mutableModel.GetObject((Enumeration)enumObject), enumBuilder);
            mutableModel.MergeObjects(mutableModel.GetObject(enumLiteral), enumLiteralBuilder);
            return mutableModel.ToImmutable();
        }

        /// <summary>
        /// Creates a new EnumerationLiteral with a random name
        /// </summary>
        /// <returns>The create EnumerationLiteral object</returns>
        private ImmutableObject CreateEnumLiteral()
        {
            var personGenerator = new PersonNameGenerator();
            var name = personGenerator.GenerateRandomFirstAndLastName();

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
        public ImmutableModel AddNewParameterToOperation(ImmutableObject operationObject)
        {
            var mutableModel = this._immutableModel.ToMutable();
            var parameter = this.CreateParameter();
            var parameterBuilder = parameter.ToMutable();
            var parameterDirection = ParameterDirectionKind.In;

            //Resolve parameter
            mutableModel.ResolveObject(parameterBuilder.MId);
            //Set parameter's type
            this.SetObjectType(parameter, operationObject.MParent);
            //Set parameter's direction
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

        /// <summary>
        /// Creates a new parameter with a random name
        /// </summary>
        /// <returns>Returns with the newly created paramter object</returns>
        private ImmutableObject CreateParameter()
        {
            var personGenerator = new PersonNameGenerator();
            var parameterName = personGenerator.GenerateRandomFirstAndLastName();

            var mutableModel = this._immutableModel.ToMutable();
            var factory = new UmlFactory(mutableModel);
            var parameterBuilder = factory.Parameter();
            parameterBuilder.MName = parameterName;
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
