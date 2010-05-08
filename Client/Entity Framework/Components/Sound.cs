using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Audio;
using System.Xml;

namespace Kannon.Components
{
    /// <summary>
    /// Component that, when attached to an entity, can play a sound from that entities location.
    /// Note: When new camera system is implemented, add 3D sound to this, perhaps?
    /// </summary>
    class Sound : Component, IContent
    {
        Dictionary<String, SoundEffect> m_Sounds;
        Dictionary<String, SoundEffectInstance> m_PlayingSounds;

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new Sound(ent, name);
        }

        public Sound(Entity ent, String name) : base(ent, name)
        {
            m_Sounds = new Dictionary<string, SoundEffect>();
            m_PlayingSounds = new Dictionary<string, SoundEffectInstance>();
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager cm)
        {
            for(int x = 0; x < m_Sounds.Count; x++)
            {
                String s = m_Sounds.ElementAt(x).Key;
                m_Sounds[s] = cm.Load<SoundEffect>(s);
            }
        }

        public void Play(String name)
        {
            if (m_PlayingSounds.ContainsKey(name))
                m_PlayingSounds[name].Resume();
            else if (m_Sounds.ContainsKey(name))
            {
                m_PlayingSounds.Add(name, m_Sounds[name].CreateInstance());
                m_PlayingSounds[name].Play();
            }
        }

        public void Pause(String name)
        {
            if (m_PlayingSounds.ContainsKey(name))
                m_PlayingSounds[name].Pause();
        }

        public void Stop(String name)
        {
            if (m_PlayingSounds.ContainsKey(name))
            {
                m_PlayingSounds[name].Stop();
                m_PlayingSounds.Remove(name);
            }
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            foreach (XmlNode child in data)
            {
                if (child.Attributes["file"] != null)
                {
                    String filename = child.Attributes["file"].Value;
                    m_Sounds.Add(filename, null);
                    foreach (XmlNode innerChild in child)
                    {
                        switch (innerChild.Attributes["action"].Value.ToLower())
                        {
                            case "play":
                                Entity.AddEvent(innerChild.Attributes["event"].Value, (o) => Play(filename));
                                break;
                            case "pause":
                                Entity.AddEvent(innerChild.Attributes["event"].Value, (o) => Pause(filename));
                                break;
                            case "stop":
                                Entity.AddEvent(innerChild.Attributes["event"].Value, (o) => Stop(filename));
                                break;
                        }
                    }
                }
            }
        }

    }
}
