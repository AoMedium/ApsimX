﻿using Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PMF.Interfaces
{
    /// <summary>
    /// Interface for managing tillering.
    /// Tillers are stored in Culms in the Leaf organ where the first Culm is the main stem and the remaining culms are the tillers.
    /// </summary>
    public interface ITilleringMethod : IModel
    {
        /// <summary> Update number of leaves for all culms </summary>
        double CalcLeafNumber();

    }
}
