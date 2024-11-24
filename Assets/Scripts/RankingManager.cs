using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerScore
{
    public string playerName;
    public int score;

    public PlayerScore(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}

public class RankingManager : MonoBehaviour
{
    private const string RankingKey = "GameRanking"; // Chave para salvar no PlayerPrefs
    public List<PlayerScore> rankingList = new List<PlayerScore>(); // Lista do ranking

    private void Awake()
    {
        LoadRanking();
    }

    // Adiciona ou atualiza a pontua��o do jogador
    public void AddOrUpdateScore(string playerName, int score)
    {
        // Verifica se o jogador j� est� no ranking
        PlayerScore existingPlayer = rankingList.FirstOrDefault(entry => entry.playerName == playerName);
        
        if (existingPlayer != null)
        {
            // Se o jogador j� existe e a nova pontua��o for maior, atualiza
            if (score > existingPlayer.score)
            {
                existingPlayer.score = score;
            }
        }
        else
        {
            // Se o jogador n�o estiver no ranking e a pontua��o for maior, adiciona
            rankingList.Add(new PlayerScore(playerName, score));
        }

        // Ordena o ranking e mant�m os 10 melhores
        rankingList = rankingList.OrderByDescending(entry => entry.score).Take(10).ToList();
        SaveRanking();
    }

    // Salva o ranking no PlayerPrefs
    private void SaveRanking()
    {
        string json = JsonUtility.ToJson(new Serialization<PlayerScore>(rankingList));
        PlayerPrefs.SetString(RankingKey, json);
        PlayerPrefs.Save();
    }

    // Carrega o ranking do PlayerPrefs
    private void LoadRanking()
    {
        if (PlayerPrefs.HasKey(RankingKey))
        {
            string json = PlayerPrefs.GetString(RankingKey);
            rankingList = JsonUtility.FromJson<Serialization<PlayerScore>>(json).ToList();
        }
    }

    // Limpa o ranking
    public void ClearRanking()
    {
        rankingList.Clear();
        PlayerPrefs.DeleteKey(RankingKey);
    }
}


// Classe para serializar listas (necess�ria para salvar no PlayerPrefs)
[System.Serializable]
public class Serialization<T>
{
    public List<T> target;

    public Serialization(List<T> target)
    {
        this.target = target;
    }

    public List<T> ToList()
    {
        return target;
    }
}
