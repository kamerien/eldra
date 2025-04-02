using UnityEngine;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class ScryfallDataFetcher : MonoBehaviour
{
    private const string SCRYFALL_API_BASE = "https://api.scryfall.com";
    private const string OUTPUT_PATH = "Assets/Resources/CardData";
    private const string TEXTURE_OUTPUT_PATH = "Assets/Resources/CardTextures";
    private const int RATE_LIMIT_DELAY = 100; // ミリ秒

    [Serializable]
    private class ScryfallCard
    {
        public string id;
        public string name;
        public string set;
        public string type_line;
        public string mana_cost;
        public string oracle_text;
        public string power;
        public string toughness;
        public Dictionary<string, string> image_uris;
        public string rarity;
        public Dictionary<string, bool> legalities;
    }

    [ContextMenu("Fetch Card Data")]
    public async void FetchCardData()
    {
        Debug.Log("カードデータの取得を開始...");
        
        using (var client = new HttpClient())
        {
            // バルクデータの取得
            var bulkData = await GetBulkData(client);
            var cards = await ProcessBulkData(client, bulkData);
            
            // フォーマット別のデータを作成
            var formats = new[] { "standard", "pioneer", "modern", "legacy", "vintage", "pauper" };
            foreach (var format in formats)
            {
                var formatCards = cards.Where(card => 
                    card.legalities.ContainsKey(format) && 
                    card.legalities[format]
                ).ToList();
                
                SaveFormatData(format, formatCards);
            }

            // 画像のダウンロード
            await DownloadCardImages(client, cards);
        }

        Debug.Log("カードデータの取得が完了しました");
    }

    private async Task<string> GetBulkData(HttpClient client)
    {
        var response = await client.GetStringAsync($"{SCRYFALL_API_BASE}/bulk-data");
        // バルクデータURIの取得処理
        return response;
    }

    private async Task<List<ScryfallCard>> ProcessBulkData(HttpClient client, string bulkDataUri)
    {
        var cards = new List<ScryfallCard>();
        var response = await client.GetStringAsync(bulkDataUri);
        cards = JsonConvert.DeserializeObject<List<ScryfallCard>>(response);
        return cards;
    }

    private void SaveFormatData(string format, List<ScryfallCard> cards)
    {
        // フォーマット別のカードデータを保存
        var formatPath = Path.Combine(OUTPUT_PATH, format);
        Directory.CreateDirectory(formatPath);

        var cardData = cards.Select(card => new
        {
            id = card.id,
            name = card.name,
            set = card.set,
            type = card.type_line,
            manaCost = card.mana_cost,
            text = card.oracle_text,
            power = card.power,
            toughness = card.toughness,
            imageUrl = card.image_uris?["normal"],
            rarity = card.rarity
        });

        File.WriteAllText(
            Path.Combine(formatPath, "cards.json"),
            JsonConvert.SerializeObject(cardData, Formatting.Indented)
        );
    }

    private async Task DownloadCardImages(HttpClient client, List<ScryfallCard> cards)
    {
        Directory.CreateDirectory(TEXTURE_OUTPUT_PATH);
        
        foreach (var card in cards)
        {
            if (card.image_uris == null || !card.image_uris.ContainsKey("normal"))
                continue;

            var imagePath = Path.Combine(TEXTURE_OUTPUT_PATH, $"{card.id}.jpg");
            if (File.Exists(imagePath))
                continue;

            try
            {
                var imageBytes = await client.GetByteArrayAsync(card.image_uris["normal"]);
                File.WriteAllBytes(imagePath, imageBytes);
                await Task.Delay(RATE_LIMIT_DELAY); // APIレート制限への対応
            }
            catch (Exception ex)
            {
                Debug.LogError($"画像のダウンロードエラー: {card.name} - {ex.Message}");
            }
        }
    }

    // アトラス生成
    private void GenerateAtlases()
    {
        // テクスチャアトラスの生成処理
        // Unity Editor API を使用してアトラスを生成
    }

    // データの最適化
    private void OptimizeData()
    {
        // テクスチャの圧縮
        // JSONデータの最適化
        // キャッシュの生成
    }
}
