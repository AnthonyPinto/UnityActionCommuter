using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public enum Layer { RailOne, ChannelOne, RailTwo, ChannelTwo, RailThree, ChannelThree, RailFour }

    public static Dictionary<Layer, string> LayerString = new Dictionary<Layer, string>()
    {
        [Layer.RailOne] = "RailOne",
        [Layer.ChannelOne] = "ChannelOne",
        [Layer.RailTwo] = "RailTwo",
        [Layer.ChannelTwo] = "ChannelTwo",
        [Layer.RailThree] = "RailThree",
        [Layer.ChannelThree] = "ChannelThree",
        [Layer.RailFour] = "RailFour",
    };

    public static List<Layer> LayerList = new List<Layer>()
    {
        Layer.RailOne,
        Layer.ChannelOne,
        Layer.RailTwo,
        Layer.ChannelTwo,
        Layer.RailThree,
        Layer.ChannelThree,
        Layer.RailFour,
    };
}
