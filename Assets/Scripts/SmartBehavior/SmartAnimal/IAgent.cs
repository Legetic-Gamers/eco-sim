using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgent
{ 
   Action<float> onEpisodeBegin { get; set; }
   Action<float> onEpisodeEnd { get; set; }

}
