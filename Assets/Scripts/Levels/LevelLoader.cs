// SCIENCE BIRDS: A clone version of the Angry Birds game used for 
// research purposes
// 
// Copyright (C) 2016 - Lucas N. Ferreira - lucasnfe@gmail.com
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>
//

﻿using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

public class LevelLoader {
    public static float PlatformMiddle;
	
	public static string ReadXmlLevel(string path) {
	
		string xmlText = "";

		if (path.Contains ("StreamingAssets")) {

			xmlText = File.ReadAllText (path);
		} 
		else {

			string[] stringSeparators = new string[] {"Levels/"};
			string[] arrayPath = path.Split (stringSeparators, StringSplitOptions.None);
			string finalPath = arrayPath [1].Split ('.') [0];

			TextAsset levelData = Resources.Load<TextAsset>("Levels/" + finalPath);
			xmlText = levelData.text;
		}

		return xmlText;
	}
	
	public static ABLevel LoadXmlLevel(string xmlString) {

		ABLevel level = new ABLevel();

		using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
		{
			reader.ReadToFollowing("Level");

			level.width = 1;
			if (reader.GetAttribute ("width") != null) {

				reader.MoveToAttribute("width");
				level.width = (int)Convert.ToInt32 (reader.Value);
			}

			reader.ReadToFollowing("Camera");

			reader.MoveToAttribute("x");
			level.camera.x = (float)Convert.ToDouble (reader.Value);

			reader.MoveToAttribute("y");
			level.camera.y = (float)Convert.ToDouble (reader.Value);

			reader.MoveToAttribute("minWidth");
			level.camera.minWidth = (float)Convert.ToDouble (reader.Value);

			reader.MoveToAttribute("maxWidth");
			level.camera.maxWidth = (float)Convert.ToDouble (reader.Value);
				
			reader.ReadToFollowing("Birds");
			reader.Read ();

			while (reader.Read ()) {

				string nodeName = reader.LocalName;
				if (nodeName == "Birds")
					break;

				reader.MoveToAttribute("type");
				string type = reader.Value;

				level.birds.Add (new BirdData (type));
				reader.Read ();
			}
            reader.ReadToFollowing("Slingshot");

			reader.MoveToAttribute("x");
			level.slingshot.x = (float)Convert.ToDouble (reader.Value);

			reader.MoveToAttribute("y");
			level.slingshot.y = (float)Convert.ToDouble (reader.Value);

			reader.ReadToFollowing("GameObjects");
			reader.Read ();

			while (reader.Read())
			{
				string nodeName = reader.LocalName;
				if (nodeName == "GameObjects")
					break;

				reader.MoveToAttribute("type");
				string type = reader.Value;

				string material = "";
				if (reader.GetAttribute ("material") != null) {

					reader.MoveToAttribute("material");
					material = reader.Value;
				}
					
				reader.MoveToAttribute("x");
				float x = (float)Convert.ToDouble(reader.Value);

				reader.MoveToAttribute("y");
				float y = (float)Convert.ToDouble(reader.Value);

				float rotation = 0f;
				if (reader.GetAttribute ("rotation") != null) {
				
					reader.MoveToAttribute ("rotation");
					rotation = (float)Convert.ToDouble (reader.Value);
				}

                if (nodeName == "Block")
                {

                    level.blocks.Add(new BlockData(type, rotation, x, y, material));
                    reader.Read();
                }
                else if (nodeName == "Pig")
                {

                    level.pigs.Add(new OBjData(type, rotation, x, y));
                    reader.Read();
                }
                else if (nodeName == "TNT")
                {

                    level.tnts.Add(new OBjData(type, rotation, x, y));
                    reader.Read();
                }
                else if (nodeName == "Platform")
                {

                    float scaleX = 1f;
                    if (reader.GetAttribute("scaleX") != null)
                    {

                        reader.MoveToAttribute("scaleX");
                        scaleX = (float)Convert.ToDouble(reader.Value);
                    }

                    float scaleY = 1f;
                    if (reader.GetAttribute("scaleY") != null)
                    {

                        reader.MoveToAttribute("scaleY");
                        scaleY = (float)Convert.ToDouble(reader.Value);
                    }

                    level.platforms.Add(new PlatData(type, rotation, x, y, scaleX, scaleY));
                    reader.Read();
                }
                else if (nodeName == "Trigger")
                {
                        level.triggerX = (float)Convert.ToDouble(x);
                        level.triggerY = (float)Convert.ToDouble(y);
                    
                    reader.Read();
                }
				else if (nodeName == "Horizontal")
				{
					if (reader.GetAttribute("bool") != null)
					{
						reader.MoveToAttribute("bool");
						level.horizontal = (bool)Convert.ToBoolean(reader.Value);
					}
					reader.Read();
				}
			}
		}

		return level;
	}

	public static void SaveXmlLevel(ABLevel level, string path,bool SymModel) 
    {
		StringBuilder output = new StringBuilder();
		XmlWriterSettings ws = new XmlWriterSettings();
		ws.Indent = true;
        string CombinePath = Path.Combine(path, "");
        if(SymModel)
            //path = Path.Combine(CombinePath, DateTime.Now.ToString("MMddyy-HHmmss")+".xml");
            path = Path.Combine(CombinePath, ("s"+LevelList.Instance.CurrentIndex).ToString() + ".xml");
        else
            path = Path.Combine(CombinePath, (LevelList.Instance.CurrentIndex).ToString() + ".xml");
		if (LevelSimulator.generateLevel)
			path = Path.Combine(CombinePath,DateTime.Now.ToString("MMddyy-HHmmss") + ".xml");
//        Debug.Log("Save XML: " + path);
		using (XmlWriter writer = XmlWriter.Create(output, ws)) {
			writer.WriteStartElement("Level");
			writer.WriteAttributeString("width", level.width.ToString());

			writer.WriteStartElement("Camera");
			writer.WriteAttributeString("x", level.camera.x.ToString());
			writer.WriteAttributeString("y", level.camera.y.ToString());
			writer.WriteAttributeString("minWidth", level.camera.minWidth.ToString());
			writer.WriteAttributeString("maxWidth", level.camera.maxWidth.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("Birds");
            foreach (BirdData abBird in level.birds) {
                writer.WriteStartElement("Bird");
                writer.WriteAttributeString("type", abBird.type.ToString());
                writer.WriteEndElement();
            }
            writer.WriteString("\n");
            writer.WriteEndElement();

            //writer.WriteStartElement("TriggerPoint");
            //writer.WriteAttributeString("x", level.triggerX.ToString());
            //writer.WriteAttributeString("y", level.triggerY.ToString());
            //writer.WriteEndElement();

            writer.WriteStartElement("Slingshot");
			writer.WriteAttributeString("x", level.slingshot.x.ToString());
			writer.WriteAttributeString("y", level.slingshot.y.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("GameObjects");

			foreach(BlockData abObj in level.blocks) {
				writer.WriteStartElement("Block");
                String[] indexType = abObj.type.ToString().Split('(');
                writer.WriteAttributeString("type", indexType[0]);
				writer.WriteAttributeString("material", abObj.material.ToString());
				writer.WriteAttributeString("x", abObj.x.ToString());
				writer.WriteAttributeString("y", abObj.y.ToString());
				writer.WriteAttributeString("rotation", abObj.rotation.ToString());
				writer.WriteEndElement();
			}

			foreach(OBjData abObj in level.pigs) {
				writer.WriteStartElement("Pig");
                String[] indexType = abObj.type.ToString().Split('(');
                writer.WriteAttributeString("type", indexType[0]);
				writer.WriteAttributeString("x", abObj.x.ToString());
				writer.WriteAttributeString("y", abObj.y.ToString());
				writer.WriteAttributeString("rotation", abObj.rotation.ToString());
				writer.WriteEndElement();
			}

			foreach(OBjData abObj in level.tnts)
			{
				writer.WriteStartElement("TNT");
                String[] indexType = abObj.type.ToString().Split('(');
                writer.WriteAttributeString("type", indexType[0]);
				writer.WriteAttributeString("x", abObj.x.ToString());
				writer.WriteAttributeString("y", abObj.y.ToString());
				writer.WriteAttributeString("rotation", abObj.rotation.ToString());
				writer.WriteEndElement();
			}

			foreach(PlatData abObj in level.platforms)
			{
				writer.WriteStartElement("Platform");
                String[] indexType = abObj.type.ToString().Split('(');
                writer.WriteAttributeString("type", indexType[0]);
				writer.WriteAttributeString("x", abObj.x.ToString());
				writer.WriteAttributeString("y", abObj.y.ToString());
				writer.WriteEndElement();
			}
			if (!LevelSimulator.generateLevel) {
				ABLevel currentLevel = LevelList.Instance.GetCurrentLevel ();
				if (currentLevel.triggerX == 0)
					return;
				/////If there is no trigger point, consider subset as useless.
				if (!SymModel) {
					writer.WriteStartElement ("Trigger");
					writer.WriteAttributeString ("type", "Trigger");
					writer.WriteAttributeString ("x", currentLevel.triggerX.ToString ());
					writer.WriteAttributeString ("y", currentLevel.triggerY.ToString ());
					writer.WriteEndElement ();

				} else {
					writer.WriteStartElement ("Trigger");
					writer.WriteAttributeString ("type", "Trigger");
					writer.WriteAttributeString ("x", (2 * PlatformMiddle - currentLevel.triggerX).ToString ());
					writer.WriteAttributeString ("y", currentLevel.triggerY.ToString ());
					writer.WriteEndElement ();
				}
				writer.WriteStartElement ("Horizontal");
				writer.WriteAttributeString ("type", "Horizontal");
				writer.WriteAttributeString ("x", "0");
				writer.WriteAttributeString ("y", "0");
				writer.WriteAttributeString ("bool", currentLevel.horizontal.ToString ());
				writer.WriteEndElement ();
			}


            writer.WriteString("\n");
            writer.WriteEndElement();
		}
		
		StreamWriter streamWriter = new StreamWriter(path);
		streamWriter.WriteLine(output.ToString());
		streamWriter.Close();
	}

	public static void SaveXmlLevel(ABLevel level, string path) {

		StringBuilder output = new StringBuilder();
		XmlWriterSettings ws = new XmlWriterSettings();
		ws.Indent = true;
		path = Path.Combine(path,DateTime.Now.ToString("MMddyy-HHmmss") + ".xml");

		using (XmlWriter writer = XmlWriter.Create(output, ws))
		{
			writer.WriteStartElement("Level");
			writer.WriteAttributeString("width", level.width.ToString());

			writer.WriteStartElement("Camera");
			writer.WriteAttributeString("x", level.camera.x.ToString());
			writer.WriteAttributeString("y", level.camera.y.ToString());
			writer.WriteAttributeString("minWidth", level.camera.minWidth.ToString());
			writer.WriteAttributeString("maxWidth", level.camera.maxWidth.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("Birds");
			foreach (BirdData abBird in level.birds) {
				writer.WriteStartElement("Bird");
				writer.WriteAttributeString("type", abBird.type.ToString());
				writer.WriteEndElement();
			}
			writer.WriteString("\n");
			writer.WriteEndElement();

			writer.WriteStartElement("Slingshot");
			writer.WriteAttributeString("x", level.slingshot.x.ToString());
			writer.WriteAttributeString("y", level.slingshot.y.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("GameObjects");

			foreach(BlockData abObj in level.blocks) {
				writer.WriteStartElement("Block");
				String[] indexType = abObj.type.ToString().Split('(');
				writer.WriteAttributeString("type", indexType[0]);
				writer.WriteAttributeString("material", abObj.material.ToString());
				writer.WriteAttributeString("x", abObj.x.ToString());
				writer.WriteAttributeString("y", abObj.y.ToString());
				writer.WriteAttributeString("rotation", abObj.rotation.ToString());
				writer.WriteEndElement();
			}

			foreach(OBjData abObj in level.pigs)
			{
				writer.WriteStartElement("Pig");
				writer.WriteAttributeString("type", abObj.type.ToString());
				writer.WriteAttributeString("x", abObj.x.ToString());
				writer.WriteAttributeString("y", abObj.y.ToString());
				writer.WriteAttributeString("rotation", abObj.rotation.ToString());
				writer.WriteEndElement();
			}

			foreach(OBjData abObj in level.tnts)
			{
				writer.WriteStartElement("TNT");
				writer.WriteAttributeString("type", abObj.type.ToString());
				writer.WriteAttributeString("x", abObj.x.ToString());
				writer.WriteAttributeString("y", abObj.y.ToString());
				writer.WriteAttributeString("rotation", abObj.rotation.ToString());
				writer.WriteEndElement();
			}

			foreach(PlatData abObj in level.platforms)
			{
				writer.WriteStartElement("Platform");
				writer.WriteAttributeString("type", abObj.type.ToString());
				writer.WriteAttributeString("x", abObj.x.ToString());
				writer.WriteAttributeString("y", abObj.y.ToString());
				writer.WriteAttributeString("rotation", abObj.rotation.ToString());
				writer.WriteAttributeString("scaleX", abObj.scaleX.ToString());
				writer.WriteAttributeString("scaleY", abObj.scaleY.ToString());
				writer.WriteEndElement();
			}
		}

		StreamWriter streamWriter = new StreamWriter(path);
		streamWriter.WriteLine(output.ToString());
		streamWriter.Close();
	}
		
	public static Dictionary<string, GameObject> LoadABResource(string path) {

		// Load block templates and cast them to game objects
		UnityEngine.Object[] objs = Resources.LoadAll(path);

		Dictionary<string, GameObject> resources = new Dictionary<string, GameObject>();

		for (int i = 0; i < objs.Length; i++) {

			GameObject abGameObject = (GameObject)objs[i];
			resources [abGameObject.name] = abGameObject;
		}

		return resources;
	}

    public ABLevel EncodeLevel() {
        ABLevel level = new ABLevel();
        level.width = LevelList.Instance.GetCurrentLevel().width;
        level.camera = LevelList.Instance.GetCurrentLevel().camera;
        level.slingshot = LevelList.Instance.GetCurrentLevel().slingshot;
        level.birds = LevelList.Instance.GetCurrentLevel().birds;
		level.horizontal = LevelList.Instance.GetCurrentLevel().horizontal;

        foreach (Transform child in GameObject.Find("Blocks").transform) 
        {
            string type = child.name;
            float x = child.transform.position.x;
            float y = child.transform.position.y;
            float rotation = child.transform.rotation.eulerAngles.z;

            if (child.GetComponent<ABPig>() != null)
            {
                level.pigs.Add(new OBjData(type, rotation, x, y));
            }
            else if (child.GetComponent<ABBlock>() != null)
            {
                string material = child.GetComponent<ABBlock>()._material.ToString();
                level.blocks.Add(new BlockData(type, rotation, x, y, material));
            }
            else if (child.GetComponent<ABTNT>() != null)
            {
                level.tnts.Add(new OBjData(type, rotation, x, y));
            }
        }

        foreach (Transform child in GameObject.Find("Platforms").transform) 
        {
            PlatData obj = new PlatData();
            obj.type = child.name;
            obj.x = child.transform.position.x;
            obj.y = child.transform.position.y;
            obj.rotation = child.transform.rotation.eulerAngles.z;
            obj.scaleX = child.transform.localScale.x;
            obj.scaleY = child.transform.localScale.y;
            level.platforms.Add(obj);
        }
        return level;
    }

    public ABLevel EncodeSymmetricalLevel()
    {
        float platformMiddlePoint = 0;
        ABLevel level = new ABLevel();
        level.width = LevelList.Instance.GetCurrentLevel().width;
        level.camera = LevelList.Instance.GetCurrentLevel().camera;
        level.slingshot = LevelList.Instance.GetCurrentLevel().slingshot;
        level.birds = LevelList.Instance.GetCurrentLevel().birds;
        level.platforms = LevelList.Instance.GetCurrentLevel().platforms;
		level.horizontal = LevelList.Instance.GetCurrentLevel().horizontal;

        foreach (Transform child in GameObject.Find("Platforms").transform)
        {
            PlatData obj = new PlatData();
            obj.type = child.name;
            obj.x = child.transform.position.x;
            obj.y = child.transform.position.y;
            obj.rotation = child.transform.rotation.eulerAngles.z;
            obj.scaleX = child.transform.localScale.x;
            obj.scaleY = child.transform.localScale.y;
            platformMiddlePoint += child.transform.position.x;
//            level.platforms.Add(obj);
        }
        platformMiddlePoint /= level.platforms.Count;
        PlatformMiddle = platformMiddlePoint;
        foreach (Transform child in GameObject.Find("Blocks").transform)
        {
            string type = child.name;
			float x = 2 * platformMiddlePoint;
			x = x - child.transform.position.x;
            float y = child.transform.position.y;
            float rotation = child.transform.rotation.eulerAngles.z;

            if (child.GetComponent<ABPig>() != null)
            {
                level.pigs.Add(new OBjData(type, rotation, x, y));
            }
            else if (child.GetComponent<ABBlock>() != null)
            {
                string material = child.GetComponent<ABBlock>()._material.ToString();
                level.blocks.Add(new BlockData(type, rotation, x, y, material));
            }
            else if (child.GetComponent<ABTNT>() != null)
            {
                level.tnts.Add(new OBjData(type, rotation, x, y));
            }
        }

        
        return level;
    }
    
    public void SaveLevelOnScene() {
        //Use this code to save objs
        ABLevel level = EncodeLevel();
        //Save level to xml file
		SaveXmlLevel(level ,Application.dataPath + ABConstants.CUSTOM_LEVELS_FOLDER, false);
	}

    //platformMiddlePoint is the centre of the platforms in current subsets
    public void SaveLevel(ABLevel level,bool SymModel)
    {
        //Save level to xml file
		SaveXmlLevel(level, Application.dataPath + ABConstants.EVALUATED_SUBSETS_FOLDER, SymModel);
    }
}