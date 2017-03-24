// generated Class
//directory=F:\GPT2016MT1\GPT2016MT1\GPT2016MT1\RCStock1
//outfile=F:\GPT2016MT1\GPT2016MT1\GPT2016MT1\RCStock1\RC_Stock1.cs

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

public static class RC_Stock1
{

        public static string dir;

            public static RC_TextureList rctexList = null;
            
                public static Texture2D texBackground3=null;
                public static Texture2D texBomb1=null;
                public static Texture2D texBoom1=null;
                public static Texture2D texBox0=null;
                public static Texture2D texBox1=null;
                public static Texture2D texBox2=null;
                public static Texture2D texBrick0=null;
                public static Texture2D texbrick1=null;
                public static Texture2D texbrick2=null;
                public static Texture2D texbrick3=null;
                public static Texture2D texbrick4=null;
                public static Texture2D texbrick5=null;
                public static Texture2D texbrick6=null;
                public static Texture2D texCloud1=null;
                public static Texture2D texCloud2=null;
                public static Texture2D texDoor0=null;
                public static Texture2D texDoor1=null;
                public static Texture2D texFan=null;
                public static Texture2D texGreyGhost=null;
                public static Texture2D texGreyGirl=null;
                public static Texture2D texGreyGuy=null;
                public static Texture2D texHeart1=null;
                public static Texture2D texLadder0=null;
                public static Texture2D texSkull=null;
                public static Texture2D texSpike=null;
                public static Texture2D texSpikes=null;
                public static Texture2D texStar2=null;
                public static Texture2D texStar3=null;
                public static Texture2D texWood1=null;
                public static Texture2D texWood2=null;
                public static Texture2D texWood3=null;

        public static void init(GraphicsDevice gd, string directory)
        {
            if (directory == "")    
            {
            directory = Util.findDirWithFile(@"Background3.png");
            dir = directory+@"\";
            }
            else
            {
            dir=directory;
            }
            
            rctexList = new RC_TextureList(gd);
            
            int idx;

            idx = rctexList.add("Background3",dir+"Background3.png");
            texBackground3= rctexList[idx].tex();

            idx = rctexList.add("Bomb1",dir+"Bomb1.png");
            texBomb1= rctexList[idx].tex();

            idx = rctexList.add("Boom1",dir+"Boom1.png");
            texBoom1= rctexList[idx].tex();

            idx = rctexList.add("Box0",dir+"Box0.png");
            texBox0= rctexList[idx].tex();

            idx = rctexList.add("Box1",dir+"Box1.png");
            texBox1= rctexList[idx].tex();

            idx = rctexList.add("Box2",dir+"Box2.png");
            texBox2= rctexList[idx].tex();

            idx = rctexList.add("Brick0",dir+"Brick0.png");
            texBrick0= rctexList[idx].tex();

            idx = rctexList.add("brick1",dir+"brick1.png");
            texbrick1= rctexList[idx].tex();

            idx = rctexList.add("brick2",dir+"brick2.png");
            texbrick2= rctexList[idx].tex();

            idx = rctexList.add("brick3",dir+"brick3.png");
            texbrick3= rctexList[idx].tex();

            idx = rctexList.add("brick4",dir+"brick4.png");
            texbrick4= rctexList[idx].tex();

            idx = rctexList.add("brick5",dir+"brick5.png");
            texbrick5= rctexList[idx].tex();

            idx = rctexList.add("brick6",dir+"brick6.png");
            texbrick6= rctexList[idx].tex();

            idx = rctexList.add("Cloud1",dir+"Cloud1.png");
            texCloud1= rctexList[idx].tex();

            idx = rctexList.add("Cloud2",dir+"Cloud2.png");
            texCloud2= rctexList[idx].tex();

            idx = rctexList.add("Door0",dir+"Door0.png");
            texDoor0= rctexList[idx].tex();

            idx = rctexList.add("Door1",dir+"Door1.png");
            texDoor1= rctexList[idx].tex();

            idx = rctexList.add("Fan",dir+"Fan.png");
            texFan= rctexList[idx].tex();

            idx = rctexList.add("GreyGhost",dir+"GreyGhost.png");
            texGreyGhost= rctexList[idx].tex();

            idx = rctexList.add("GreyGirl",dir+"GreyGirl.png");
            texGreyGirl= rctexList[idx].tex();

            idx = rctexList.add("GreyGuy",dir+"GreyGuy.png");
            texGreyGuy= rctexList[idx].tex();

            idx = rctexList.add("Heart1",dir+"Heart1.png");
            texHeart1= rctexList[idx].tex();

            idx = rctexList.add("Ladder0",dir+"Ladder0.png");
            texLadder0= rctexList[idx].tex();

            idx = rctexList.add("Skull",dir+"Skull.png");
            texSkull= rctexList[idx].tex();

            idx = rctexList.add("Spike",dir+"Spike.png");
            texSpike= rctexList[idx].tex();

            idx = rctexList.add("Spikes",dir+"Spikes.png");
            texSpikes= rctexList[idx].tex();

            idx = rctexList.add("Star2",dir+"Star2.png");
            texStar2= rctexList[idx].tex();

            idx = rctexList.add("Star3",dir+"Star3.png");
            texStar3= rctexList[idx].tex();

            idx = rctexList.add("Wood1",dir+"Wood1.png");
            texWood1= rctexList[idx].tex();

            idx = rctexList.add("Wood2",dir+"Wood2.png");
            texWood2= rctexList[idx].tex();

            idx = rctexList.add("Wood3",dir+"Wood3.png");
            texWood3= rctexList[idx].tex();

     }
    }
}
