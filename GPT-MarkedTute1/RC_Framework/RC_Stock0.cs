// generated Class
//directory=F:\B\RC_Framework\StockArt\RCStock0\
//outfile=F:\B\RC_Framework\Source\RC_Stock0.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RC_Framework
{

public static class RC_Stock0
{

        public static string dir;

            public static RC_TextureList rctexList = null;
            
                public static Texture2D texBackground0=null;
                public static Texture2D texBackground1=null;
                public static Texture2D texBackground2=null;
                public static Texture2D texBomb0=null;
                public static Texture2D texBoom0=null;
                public static Texture2D texBubble0=null;
                public static Texture2D texCloud0=null;
                public static Texture2D texGreenGuy0=null;
                public static Texture2D texGreenGuy1=null;
                public static Texture2D texHeart0=null;
                public static Texture2D texHouse0=null;
                public static Texture2D texMouse0=null;
                public static Texture2D texStar0=null;
                public static Texture2D texTree0=null;

        public static void init(GraphicsDevice gd, string directory)
        {
            if (directory == "")    
            {
            directory = Util.findDirWithFile(@"RCStock0\texBackground0.png");
            dir = directory+@"RCStock0\";
            }
            else
            {
            dir=directory;
            }
            
            rctexList = new RC_TextureList(gd);
            
            int idx;

            idx = rctexList.add("Background0",dir+"texBackground0.png");
            texBackground0= rctexList[idx].tex();

            idx = rctexList.add("Background1",dir+ "texBackground1.png");
            texBackground1= rctexList[idx].tex();

            idx = rctexList.add("Background2",dir+ "texBackground2.png");
            texBackground2= rctexList[idx].tex();

            idx = rctexList.add("Bomb0",dir+ "texBomb0.png");
            texBomb0= rctexList[idx].tex();

            idx = rctexList.add("Boom0",dir+ "texBoom0.png");
            texBoom0= rctexList[idx].tex();

            idx = rctexList.add("Bubble0",dir+ "texBubble0.png");
            texBubble0= rctexList[idx].tex();

            idx = rctexList.add("Cloud0",dir+ "texCloud0.png");
            texCloud0= rctexList[idx].tex();

            idx = rctexList.add("GreenGuy0",dir+ "texGreenGuy0.png");
            texGreenGuy0= rctexList[idx].tex();

            idx = rctexList.add("GreenGuy1",dir+ "texGreenGuy1.png");
            texGreenGuy1= rctexList[idx].tex();

            idx = rctexList.add("Heart0",dir+ "texHeart0.png");
            texHeart0= rctexList[idx].tex();

            idx = rctexList.add("House0",dir+ "texHouse0.png");
            texHouse0= rctexList[idx].tex();

            idx = rctexList.add("Mouse0",dir+ "texMouse0.png");
            texMouse0= rctexList[idx].tex();

            idx = rctexList.add("Star0",dir+ "texStar0.png");
            texStar0= rctexList[idx].tex();

            idx = rctexList.add("Tree0",dir+ "texTree0.png");
            texTree0= rctexList[idx].tex();

     }
    }
}
