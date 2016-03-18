using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.TreeScripts
{
    class ScriptScale
    {
        //this sets up the over all scaling of our world
        public float PersonWidth = 50.0f;  // The width of a person platform
        public float InterPersonSpacing = 5.0f;  // The spacing between person platforms
        public float InterHouseSpacing = 20.0f;  // The spacing between Houses
        public float ZScale = 2.0f;  // Number of meters per year
        public float GenerationGap = 200.0f;   // Y axis offset for each generation
    }
}
