// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

/*
 * TODO:
 * 1. Animated billboards
 * 2. Dungeon enemies
 * 3. Exterior billboards
 * 4. PaperDoll CharacterLayer textures works only if resolution is the same as vanilla 
         (http://forums.dfworkshop.net/viewtopic.php?f=22&p=3547&sid=6a99dbcffad1a15b08dd5e157274b772#p3547)
 */

using System.IO;
using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    static public class DFTextureReplacement
    {
        /// <summary>
        /// Load custom image files from disk to use as textures on models or billboards
        /// .png files are located in persistentData/textures
        /// and are named 'archive_record-frame.png' 
        /// for example '86_3-0.png'
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards</param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomTextureExist(int archive, int record, int frame)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + ".png"))
                return true;

            return false;
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomTexture(int archive, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + ".png"));

            return tex; //assign image to the actual texture
        }

        /// import custom image as texture2D for Billboards
        /// This works only for non-animated interior and dungeon billboards for now
        static public void LoadCustomBillboardTexture(int archive, int record, ref GameObject go)
        {
            // Main texture
            go.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", LoadCustomTexture(archive, record, 0));
            go.GetComponent<Renderer>().materials[0].mainTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;

            // Emission map
            // Import emission map
            if (CustomEmissionExist(archive, record, 0))
            {
                Texture2D emissionMap = LoadCustomEmission(archive, record, 0);
                emissionMap.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                go.GetComponent<Renderer>().materials[0].SetTexture("_EmissionMap", emissionMap);
            }
            // If texture is emissive but no emission map is provided, emits from the whole surface
            else if (go.GetComponent<Renderer>().materials[0].GetTexture("_EmissionMap") != null)
                go.GetComponent<Renderer>().materials[0].SetTexture("_EmissionMap", go.GetComponent<Renderer>().materials[0].GetTexture("_MainTex"));

            // Create new UV map
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 1);
            uv[1] = new Vector2(1, 1);
            uv[2] = new Vector2(0, 0);
            uv[3] = new Vector2(1, 0);
            go.GetComponent<MeshFilter>().mesh.uv = uv;
        }

        /// <summary>
        /// Load custom image files from disk to replace .IMGs. Useful for customizing the UI
        /// .png files are located in persistentdata/textures/img
        /// and are named 'imagefile.png' 
        /// for example 'REST02I0.IMG.png'
        /// </summary>
        /// <param name="filename">Name of standalone file as it appears in arena2 folder.</param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomImageExist(string filename)
        {

            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Application.persistentDataPath + "/textures/img/" + filename + ".png"))
                return true;

            return false;
        }

        /// load custom image as texture2D
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomImage(string filename)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/img/" + filename + ".png"));

            return tex; //assign image to the actual texture
        }

        /// <summary>
        /// Load custom image files from disk to replace .CIFs and .RCIs
        /// .png files are located in persistentdata/textures/cif
        /// and are named 'CifFile_record-frame.png' 
        /// for example 'INVE16I0.CIF_1-0.png'
        /// </summary>
        /// <param name="filename">Name of standalone file as it appears in arena2 folder.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for weapon animations (WEAPONXX.CIF) </param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomCifExist(string filename, int record, int frame)
        {

            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Application.persistentDataPath + "/textures/cif/" + filename + "_" + record.ToString() + "-" + frame.ToString() + ".png"))
                return true;

            return false;
        }

        /// load custom image as texture2D
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomCif(string filename, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/cif/" + filename + "_" + record.ToString() + "-" + frame.ToString() + ".png"));

            return tex; //assign image to the actual texture
        }

        /// <summary>
        /// Load custom image files from disk to use as normal maps
        /// .png files are located in persistentData/textures
        /// and are named 'archive_record-frame_Normal.png' 
        /// for example '112_3-0_Normal.png'
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards</param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomNormalExist(int archive, int record, int frame)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Normal.png"))
                return true;

            return false;
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomNormal(int archive, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, true); //create empty texture, size will be the actual size of .png file
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Normal.png"));

            Color32[] colours = tex.GetPixels32();
            for (int i = 0; i < colours.Length; i++)
            {
                colours[i].a = colours[i].r;
                colours[i].r = colours[i].b = colours[i].g;
            }
            tex.SetPixels32(colours);
            tex.Apply();

            return tex;
        }

        /// <summary>
        /// Load custom image files from disk to use as emission maps
        /// This is useful for walls, where only the windows emits light
        /// .png files are located in persistentData/textures
        /// and are named 'archive_record-frame_Emission.png' 
        /// for example '112_3-0_Emission.png'
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards</param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomEmissionExist(int archive, int record, int frame)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Emission.png"))
                return true;

            return false;
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomEmission(int archive, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Emission.png"));

            return tex; //assign image to the actual texture
        }
    }
}