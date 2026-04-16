using System;
using System.Collections.Generic;

namespace QAI
{
    [Serializable]
    public class UserProfileAssetDto
    {
        public List<AssetItemDto> assets;
    }

    [Serializable]
    public class AssetItemDto
    {
        public TokenDto token;
    }

    [Serializable]
    public class TokenDto
    {
        public int ID;
        public string Title;
        public string Ticker;

        public int GameID;

        public double PriceUsd;
        public double MarketCap;
        public string Description;
        public string Status;
        public string MintAddress;
    }
}