using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace Kannon
{
    /// <summary>
    /// Definition of a set (group of entities)
    /// </summary>
    public struct SetDefinition : ICloneable
    {
        public String Name;
        public List<XmlNode> Entities;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the set.</param>
        public SetDefinition(String name)
        {
            Name = name;
            Entities = new List<XmlNode>();
        }

        /// <summary>
        /// Zips together the current set along with some additional customization data.
        /// </summary>
        /// <param name="customization">Data to customize the set definition with.</param>
        /// <returns>A completely new SetDefinition object, customized based on the customization parameters</returns>
        /// <remarks>
        /// Structure of a Set:
        ///     <Set name="testSet">
        ///         <Entity base="Test" type="add"/>
        ///         <Entity base="Test" type="modify">
        ///             <Property name="Position" type="remove"/>
        ///             <Component type="Physics" type="add"/>
        ///         </Entity>
        ///     </Set>
        ///     <Set name="Test2" base="testSet">
        ///         <Entity base="Test" type="remove"/>
        ///         <Entity base="Rawr" type="add"/>
        ///     </Set>
        /// </remarks>
        public static SetDefinition ZipTogether(SetDefinition set, XmlNode customization)
        {
            //Clone the set so we don't end up modifying the original.
            SetDefinition newSet = (SetDefinition)set.Clone();
            // If we aren't to do any customization, just return.
            if (customization != null)
            {
                // For each of the customization points,
                foreach (XmlNode child in customization.ChildNodes)
                {
                    // Make sure it's an Entity (sets don't really understand anything else yet)
                    if (child.Name.ToLower() == "entity" && child.Attributes["mod"] != null)
                    {
                        // Check what modification type we are, add remove or modify.
                        switch (child.Attributes["mod"].Value.ToLower())
                        {
                            case "add":
                                newSet.Entities.Add(child);
                                break;
                            case "remove":
                                if (child.Attributes["name"] != null)
                                {
                                    String name = child.Attributes["name"].Value;
                                    newSet.Entities.RemoveAll((node) => node.Attributes["name"] != null && node.Attributes["name"].Value == name);
                                }
                                break;
                            case "modify":
                                if (child.Attributes["name"] != null)
                                {
                                    // Zipping together the inner XML with what we have stored creates a new customization point, which will later be applied to entities.
                                    String name = child.Attributes["name"].Value;
                                    foreach (XmlNode node in newSet.Entities)
                                    {
                                        if( node.Attributes["name"] != null && node.Attributes["name"].Value == name)
                                            node.InnerXml = node.ZipTogether(child).InnerXml;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            return newSet;
        }

        /// <summary>
        /// Clone this set.
        /// </summary>
        /// <returns>A cloned set.</returns>
        public object Clone()
        {
            SetDefinition newSet = new SetDefinition(this.Name);
            foreach(XmlNode node in this.Entities)
                newSet.Entities.Add(node.Clone());
            return newSet;
        }
    }

    /// <summary>
    /// Definition of an entity (group of properties and components).
    /// </summary>
    public struct EntityDefinition : ICloneable
    {
        public String Name;
        public List<XmlNode> m_Components;
        public List<XmlNode> m_Properties;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the entity.</param>
        public EntityDefinition(String name)
        {
            Name = name;
            m_Components = new List<XmlNode>();
            m_Properties = new List<XmlNode>();
        }

        /// <summary>
        /// Convert this entity definition to XML
        /// </summary>
        /// <returns>The entity definition written to XML format.</returns>
        public XmlNode ToXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("Entity");
            node.Attributes.Append( doc.CreateAttribute("name") );
            node.Attributes["name"].Value = Name;
            foreach (XmlNode prop in m_Properties)
            {
                XmlNode newNode = doc.ImportNode(prop, true);
                node.AppendChild(newNode);
            }
            foreach (XmlNode comp in m_Components)
            {
                XmlNode newNode = doc.ImportNode(comp, true);
                node.AppendChild(newNode);
            }
            return node;
        }

        /// <summary>
        /// Customize a specific entity definition with some data.
        /// </summary>
        /// <param name="ent">Base entity to work with.</param>
        /// <param name="customizationData">Data to customize.</param>
        /// <returns>A customized entity.</returns>
        public static EntityDefinition ZipTogether(EntityDefinition ent, XmlNode customizationData)
        {
            // Clone the entity so we don't accidentaly modify the original.
            EntityDefinition newEnt = (EntityDefinition)ent.Clone();
            // If there's no customization data, we're done here, so just return.
            if (customizationData != null)
            {
                // Otherwise, modify first the entities name
                if( customizationData.Attributes["name"] != null )
                    newEnt.Name = customizationData.Attributes["name"].Value;
                else
                    newEnt.Name = "Nameless " + newEnt.Name + new Random().Next(); // This will make sure (to reasonalbe accuracy) entity names are unique.
                // For each point of customization
                foreach (XmlNode comp in customizationData)
                {
                    // Check it's mod type
                    switch (comp.Attributes["mod"].Value.ToLower())
                    {
                        case "add":
                            // And check if it's component or property.
                            if (comp.Name.ToLower() == "component")
                            {
                                XmlNode toAdd = comp.Clone();
                                toAdd.Attributes.Remove(toAdd.Attributes["mod"]);
                                newEnt.m_Components.Add(toAdd);
                            }
                            else if (comp.Name.ToLower() == "property")
                            {
                                XmlNode toAdd = comp.Clone();
                                toAdd.Attributes.Remove(toAdd.Attributes["mod"]);
                                newEnt.m_Properties.Add(toAdd);
                            }
                            break;
                        case "remove":
                            String name = comp.Attributes["name"].Value;
                            if (comp.Name.ToLower() == "component")
                            {
                                newEnt.m_Components.RemoveAll((node) => node.Attributes["name"].Value == name);
                            }
                            else if (comp.Name.ToLower() == "property")
                            {
                                newEnt.m_Properties.RemoveAll((node) => node.Attributes["name"].Value == name);
                            }
                            break;
                        case "modify":
                            if (comp.Attributes["name"] != null)
                            {
                                name = comp.Attributes["name"].Value;
                                if (comp.Name.ToLower() == "component")
                                {
                                    // If we're modifying a component or property, we zip together the inner XML's of each of these nodes.
                                    foreach (XmlNode node in newEnt.m_Components)
                                    {
                                        if (node.Attributes["name"] != null && node.Attributes["name"].Value == name)
                                            node.InnerXml = node.ZipTogether(comp).InnerXml;
                                    }
                                }
                                else if (comp.Name.ToLower() == "property")
                                {
                                    foreach (XmlNode node in newEnt.m_Properties)
                                    {
                                        if (node.Attributes["name"] != null && node.Attributes["name"].Value == name)
                                            node.InnerXml = node.ZipTogether(comp).InnerXml;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return newEnt;
        }

        /// <summary>
        /// Clone this entity definition.
        /// </summary>
        /// <returns>Cloned entity definition.</returns>
        public object Clone()
        {
            EntityDefinition ent = new EntityDefinition(this.Name);
            foreach(XmlNode node in m_Components)
                ent.m_Components.Add(node.Clone());
            foreach(XmlNode node in m_Properties)
                ent.m_Properties.Add(node.Clone());
            return ent;
        }
    }

    /// <summary>
    /// Class for parsing, storing, and producing entities based on an XML definition.
    /// </summary>
    public static class EntityFactory
    {
        static Dictionary<String, SetDefinition> m_Sets;
        static Dictionary<String, EntityDefinition> m_Entities;

        /// <summary>
        /// Sets that have been defined.
        /// </summary>
        public static Dictionary<String, SetDefinition> Sets
        {
            get
            {
                return m_Sets;
            }
        }

        /// <summary>
        /// Entities that have been defined.
        /// </summary>
        public static Dictionary<string, EntityDefinition> Entities
        {
            get
            {
                return m_Entities;
            }
        }

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static EntityFactory()
        {
            m_Sets = new Dictionary<string, SetDefinition>();
            m_Entities = new Dictionary<string, EntityDefinition>();
        }

        /// <summary>
        /// Parses and XML Node or File recursively and constructs entity definitions.
        /// </summary>
        /// <param name="data">node to analyze</param>
        /// <remarks>
        /// Files should be structured as such:
        /// 
        /// <Project>
        ///     <Set name="testSet">
        ///         <Entity name="new" base="Test"/>
        ///         <Entity name="new2" base="Test">
        ///             <Property name="Position" remove="true"/>
        ///             <Component type="Physics" add="true"/>
        ///         </Entity>
        ///     </Set>
        ///     <Entity name="Test">
        ///         <Property type="Vector2" name="Position">
        ///             <Vector2> //XML serialized stuff
        ///             </Vector2>
        ///         </Property>
        ///         <Component type="StaticRenderable">
        ///             <StaticRenderable> // FirstChild gets passed to the Component.Initialize() or whatever
        ///             </StaticRenderable>
        ///         </Component>
        ///     </Entity>
        ///     <Entity base="Test" name="TestRawr">
        ///         
        ///     </Entity>
        /// </Project>
        /// 
        /// </remarks>
        public static void Parse(XmlNode data)
        {
            // This is a generic parse method, it determines what it's parsing, and
            switch (data.Name.ToLower().Trim())
            {
                case "project":
                    // if it's a project, just parse each child node.
                    foreach (XmlNode node in data.ChildNodes)
                        Parse(node);
                    break;
                case "set":
                    // If it's a set, just parse the set and add it to our set collection.
                    SetDefinition set = ParseSet(data);
                    m_Sets.Add(set.Name, set);
                    break;
                case "entity":
                    // Same deal with entity.
                    EntityDefinition ent = ParseEntity(data);
                    m_Entities.Add(ent.Name, ent);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Parse XML and use it to produce a SetDefinition.
        /// </summary>
        /// <param name="data">Data for the set definition.</param>
        /// <returns>The Constructed Set.</returns>
        public static SetDefinition ParseSet(XmlNode data)
        {
            // If this set says it's based on another set, and that set exists
            SetDefinition set;
            if (data.Attributes["base"] != null && m_Sets.ContainsKey(data.Attributes["base"].Value))
            {
                // Zip that set together with the passed in data.
                set = SetDefinition.ZipTogether(m_Sets[data.Attributes["base"].Value], data);
            }
            else
            {
                // Otherwise, we can just make it from scratch
                set = new SetDefinition(data.Attributes["name"].Value);

                // and add each entity node to the set.  The entities will get resolved/zipped when you
                // actually produce the set.
                foreach (XmlNode node in data.ChildNodes)
                {
                    set.Entities.Add(node);
                }
            }

            // And return the newly created/modified set definition.
            return set;

        }

        /// <summary>
        /// Parse XML and produce an EntityDefinition from it.
        /// </summary>
        /// <param name="data">Data for the entity definition.</param>
        /// <returns>Constructed entity definition.</returns>
        public static EntityDefinition ParseEntity(XmlNode data)
        {
            EntityDefinition ent;
            // If the data says it's based on another unit, and the unit it's supposedly based off of exists,
            if (data.Attributes["base"] != null && m_Entities.ContainsKey(data.Attributes["base"].Value))
            {
                // Zip that unit together with the data we were passed.
                ent = EntityDefinition.ZipTogether(m_Entities[data.Attributes["base"].Value], data);
            }
            else
            {
                // Otherwise, we're working from scratch
                ent = new EntityDefinition(data.Attributes["name"].Value);
                
                // For each of the children, determine if it's a property or a component, 
                // and add the child node to the proper list.
                foreach (XmlNode node in data.ChildNodes)
                {
                    switch (node.Name.ToLower())
                    {
                        case "property":
                            ent.m_Properties.Add(node);
                            break;
                        case "component":
                            ent.m_Components.Add(node);
                            break;
                        default:
                            break;
                    }
                }
            }

            // And return the parsed entity definition
            return ent;
        }

        /// <summary>
        /// Produces either a Set or a single Entity.  Because things
        /// automatically tie themselves up to managers and such, the entity
        /// need not be returned.
        /// </summary>
        /// <param name="name">Name of the entity/set to produce</param>
        /// <param name="customizationData">Data to customize the entity or set with.</param>
        public static void Produce(String name, XmlNode customizationData = null)
        {
            // Simply check to see if we have an entity or a set by that name, and pass it on to the more specialized functions.
            if (m_Entities.ContainsKey(name))
                ProduceEntity(name, customizationData);
            else if (m_Sets.ContainsKey(name))
                ProduceSet(name, customizationData);
            else
                throw new Exception("Entity or Set not defined.");
        }

        /// <summary>
        /// Produce an entity with given name.
        /// </summary>
        /// <param name="entityName">Name to construct the entity around</param>
        /// <param name="customizationData">Data to customize the entity with.</param>
        /// <returns>Entity produced based on a definition.</returns>
        public static Entity ProduceEntity(String entityName, XmlNode customizationData = null)
        {
            // First, check to make sure we have a definition for that entity.
            if (m_Entities.ContainsKey(entityName))
            {
                // Extract the data (and possibly zip it together with the customization data.
                Entity ent = new Entity();
                EntityDefinition entDef = EntityDefinition.ZipTogether(m_Entities[entityName], customizationData);
                // Now, for each property
                foreach (XmlNode node in entDef.m_Properties)
                {
                    // Make sure it has a specified type
                    if( node.Attributes["type"] != null )
                    {
                        // And get that type
                        Type t = Type.GetType(node.Attributes["type"].Value);
                        if (t != null)
                        {
                            // Then, construct a Property<T> based on the type above.
                            Type propType = typeof(Property<>);
                            propType = propType.MakeGenericType(t);
                            // And invoke the initializer for that (with default params).
                            IProperty prop = (IProperty)System.Activator.CreateInstance(propType);
                            // Pass it the XML node to parse,
                            prop.Parse(node.FirstChild);
                            // And add it to the entity.
                            ent.AddIProperty(node.Attributes["name"].Value, prop);
                        }
                    }
                }
                // Now, Components
                foreach (XmlNode node in entDef.m_Components)
                {
                    // If it has a type, Add that to the entity, and then making it parse first child.
                    if (node.Attributes["type"] != null)
                    {
                        String compType = node.Attributes["type"].Value;
                        String compName = (node.Attributes["name"] == null) ? 
                                            "" : 
                                            node.Attributes["name"].Value;
                        Component c = ent.AddComponent(compType, compName);
                        c.Parse(node);
                    }
                }
                return ent;
            }
            return null;
        }

        /// <summary>
        /// Produce a set from a definition.  This produces each of the entities as well.
        /// </summary>
        /// <param name="setName">Name of the set to produce.</param>
        /// <param name="customizationData">Data to customize the set with.</param>
        /// <returns>List of produced entities.</returns>
        public static List<Entity> ProduceSet(String setName, XmlNode customizationData = null)
        {
            if (m_Sets.ContainsKey(setName))
            {
                // Customize the existing set data with the customizationData
                SetDefinition set = SetDefinition.ZipTogether(m_Sets[setName], customizationData);
                List<Entity> entList = new List<Entity>();
                // And for each of the entities in it's Entities list,
                foreach (XmlNode node in set.Entities)
                {
                    // Produce the base entity, using the stored node as customization data.
                    entList.Add(ProduceEntity(node.Attributes["base"].Value, node));
                }
                // And return that list.
                return entList;
            }
            return null;
        }

        /// <summary>
        /// Zips two XmlNode's together.
        /// </summary>
        /// <param name="node">Base node to work with.</param>
        /// <param name="customization">Customization Data.</param>
        /// <returns>A zipped (merged) xml node.</returns>
        public static XmlNode ZipTogether(this XmlNode node, XmlNode customization)
        {
            // Clone it, otherwise we get weird artifacts in the calling node.
            XmlNode ret = node.CloneNode(true);
            // For each of the customization points
            foreach (XmlNode child in customization.ChildNodes)
            {
                // If there's no mod attribute, remove any preexisting nodes of the same name, and add this one.
                if (child.Attributes["mod"] == null)
                {
                    List<XmlNode> removes = new List<XmlNode>();
                    foreach (XmlNode rem in ret)
                        if (rem.Name == child.Name)
                            removes.Add(rem);
                    foreach (XmlNode rem in removes)
                        ret.RemoveChild(rem);


                    XmlNode toAdd = child.Clone();
                    ret.AppendChild(toAdd);
                    break;
                }
                else
                {
                    // Switch based on the mod type
                    switch (child.Attributes["mod"].Value)
                    {
                        case "add":
                            XmlNode toAdd = child.Clone();
                            // Make sure the destination node doesn't have a mod attribute.
                            toAdd.Attributes.Remove(toAdd.Attributes["mod"]);
                            ret.AppendChild(toAdd);
                            break;
                        case "remove":
                            List<XmlNode> removes = new List<XmlNode>();
                            foreach (XmlNode rem in ret)
                                if (rem.Attributes["name"].Value == child.Attributes["name"].Value)
                                    removes.Add(rem);
                            foreach (XmlNode rem in removes)
                                ret.RemoveChild(rem);
                            break;
                        case "modify":
                            // Zip the XML of their's together, and make their inner text equal.
                            foreach (XmlNode mod in ret)
                                if (mod.Attributes["name"].Value == child.Attributes["name"].Value)
                                    mod.InnerXml = mod.ZipTogether(child).InnerXml;
                            break;
                    }
                }
            }
            return ret;
        }
    }
}
