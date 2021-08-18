using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.Utilities
{
    public class PhysicsUtility {

        /// <summary>
        /// Return if the object is in a specific layer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            int objLayerMask = 1 << obj.layer;
            return (layerMask.value & objLayerMask) > 0;
        }
    }
}
