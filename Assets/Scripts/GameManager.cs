using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// Gerenciador principal do jogo, controla o estado geral, interface do usuário e lógica de jogo.
public class GameManager : MonoBehaviour
{
    // Lista de objetos Mole no jogo
    [SerializeField] private List<Mole> moles;

    [Header("Objetos de UI")]
    [SerializeField] private GameObject playButton;      // Botão de iniciar o jogo
    [SerializeField] private GameObject gameUI;          // Interface de jogo
    [SerializeField] private GameObject startPanel;      // Painel de início
    [SerializeField] private TMPro.TextMeshProUGUI timeText;  // Texto do tempo restante
    [SerializeField] private TMPro.TextMeshProUGUI scoreText; // Texto da pontuação
    [SerializeField] private GameObject endBombPanel;     // Painel de fim de jogo (bomba explodiu)
    [SerializeField] private GameObject endTimePanel;     // Painel de fim de jogo (tempo acabou)
    [SerializeField] private GameObject restartButton;    // Botão de reiniciar o jogo
    [SerializeField] private GameObject returnToMenuButton; // Botão de voltar ao menu

    [Header("Ranking UI")]
    [SerializeField] private RankingManager rankingManager; // Referência ao RankingManager
    [SerializeField] private TMP_InputField playerNameInput; // Campo para o nome do jogador
    private string playerName = "Jogador"; // Nome padrão do jogador
    [SerializeField] private GameObject rankingPanel; // Painel do ranking
    [SerializeField] private TextMeshProUGUI rankingText; // Campo de texto do ranking

    [Header("Sobre UI")]
    [SerializeField] private GameObject aboutPanel; // Painel de informações sobre o jogo

    [Header("Sons")]
    [SerializeField] private AudioClip gameOverSound; // Som de fim de jogo
    [SerializeField] private AudioClip backgroundSound; // Som de fundo
    [SerializeField] private AudioSource audioSource; // Fonte de áudio para tocar os sons

    // Variáveis configuráveis (ajustáveis no código)
    private float startingTime = 30f; // Tempo inicial do jogo

    // Variáveis globais
    private float timeRemaining; // Tempo restante
    private HashSet<Mole> currentMoles = new HashSet<Mole>(); // Moles atualmente ativos
    private int score; // Pontuação do jogador
    private bool playing = false; // Indica se o jogo está em andamento

    // Ocultar endBombPanel e endTimePanel no início
    void Start()
    {
        endBombPanel.SetActive(false);
        endTimePanel.SetActive(false);
        rankingPanel.SetActive(false);
        aboutPanel.SetActive(false);

        // Obtém o componente AudioSource do GameObject se não tiver sido atribuído
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    /// Método público para iniciar o jogo, chamado pelo botão de "Play".
    /// Método público para iniciar o jogo, chamado pelo botão de "Play".
    public void StartGame()
    {
        // Captura o nome do jogador ou usa um nome padrão
        playerName = string.IsNullOrWhiteSpace(playerNameInput.text) ? "Jogador" : playerNameInput.text.Trim();

        // Ocultar o painel de início
        startPanel.SetActive(false);

        // Configurar interface do usuário
        playButton.SetActive(false);
        gameUI.SetActive(true);

        // Iniciar o som de fundo
        audioSource.PlayOneShot(backgroundSound);

        // Ocultar todos os moles e configurar seus índices
        for (int i = 0; i < moles.Count; i++)
        {
            moles[i].Hide();
            moles[i].SetIndex(i);
        }

        // Resetar o estado do jogo
        currentMoles.Clear();
        timeRemaining = startingTime;
        score = 0;
        scoreText.text = "0";
        playing = true;
    }


    /// Finaliza o jogo, exibindo a mensagem apropriada.
    public void GameOver(int type)
    {
        // Parar o som de fundo
        audioSource.Stop();

        // Toca o som de fim de jogo
        audioSource.PlayOneShot(gameOverSound);

        if (type == 0)
        {
            endTimePanel.SetActive(true);  // Exibe o painel de fim de jogo por tempo
        }
        else
        {
            endBombPanel.SetActive(true);  // Exibe o painel de fim de jogo por bomba
        }

        // Salvar a pontuação no ranking
        SaveScore();

        // Ativar o botão de reiniciar e o botão de voltar ao menu
        restartButton.SetActive(true);  // Torna o botão de reiniciar visível
        returnToMenuButton.SetActive(true); // Torna o botão de voltar ao menu visível

        // Parar todos os moles
        foreach (Mole mole in moles)
        {
            mole.StopGame();
        }

        // Finalizar o jogo e mostrar o botão de reinício
        playing = false;
        playButton.SetActive(true);
    }

    /// Salva a pontuação do jogador no sistema de ranking.
    private void SaveScore()
    {
        if (rankingManager != null)
        {
            rankingManager.AddOrUpdateScore(playerName, score);  // Usando o novo método
        }
        else
        {
            Debug.LogWarning("RankingManager não está atribuído!");
        }
    }

    /// Atualiza o estado do jogo a cada frame.
    void Update()
    {
        if (playing)
        {
            // Atualizar o tempo restante
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                GameOver(0); // Fim de jogo por tempo
            }
            timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";

            // Verificar se é necessário ativar mais moles
            if (currentMoles.Count <= (score / 10))
            {
                int index = Random.Range(0, moles.Count);
                if (!currentMoles.Contains(moles[index]))
                {
                    currentMoles.Add(moles[index]);
                    moles[index].Activate(score / 10);
                }
            }
        }
    }


    /// Incrementa a pontuação e atualiza o estado do jogo ao acertar um mole.
    public void AddScore(int moleIndex)
    {
        score += 1;
        scoreText.text = $"{score}";
        timeRemaining += 1; // Incrementa o tempo restante
        currentMoles.Remove(moles[moleIndex]); // Remove o mole da lista de ativos
    }

    /// Trata o caso de erro ou falha ao clicar em um mole.
    public void Missed(int moleIndex, bool isMole)
    {
        if (isMole)
        {
            timeRemaining -= 2; // Penalidade no tempo por erro
        }
        currentMoles.Remove(moles[moleIndex]); // Remove o mole da lista de ativos
    }

    /// Reinicia o jogo (botão de reiniciar)
    public void RestartGame()
    {
        // Ocultar os painéis de fim de jogo, o botão de reiniciar e o botão de voltar ao menu
        endBombPanel.SetActive(false);
        endTimePanel.SetActive(false);
        restartButton.SetActive(false);
        returnToMenuButton.SetActive(false);

        // Iniciar o jogo novamente
        StartGame();
    }

    /// Volta para o menu principal e encerra o jogo atual (botão "Voltar ao Menu")
    public void ReturnToMenu()
    {
        // Parar todos os moles
        foreach (Mole mole in moles)
        {
            mole.StopGame();
        }

        // Resetar o estado do jogo
        currentMoles.Clear();
        score = 0;
        scoreText.text = "0";
        timeRemaining = startingTime;
        playing = false;

        // Ocultar a interface de jogo
        gameUI.SetActive(false);

        // Exibir o painel de início (menu principal)
        startPanel.SetActive(true);

        // Esconder o painel de fim de jogo, botão de reiniciar e botão de voltar ao menu
        endBombPanel.SetActive(false);
        endTimePanel.SetActive(false);
        restartButton.SetActive(false);
        returnToMenuButton.SetActive(false);

        // Exibir o botão "Play" para reiniciar o jogo
        playButton.SetActive(true);
    }

    // Exibir o painel de ranking
    public void ShowRanking()
    {
        // Limpa o texto de ranking antes de exibir novamente
        rankingText.text = "";

        // Atualiza o texto do ranking com as pontuações
        UpdateRankingText();

        // Exibe o painel de ranking
        rankingPanel.SetActive(true);

        // Oculta o menu inicial
        startPanel.SetActive(false);
    }

    // Atualiza o texto do ranking com os dados do RankingManager
    private void UpdateRankingText()
    {
        if (rankingManager != null)
        {
            var ranking = rankingManager.rankingList;
            if (ranking.Count == 0)
            {
                rankingText.text += "Nenhum jogador registrado.";
            }
            else
            {
                for (int i = 0; i < ranking.Count; i++)
                {
                    rankingText.text += $"{i + 1}. {ranking[i].playerName} - {ranking[i].score} pontos\n";
                }
            }
        }
        else
        {
            rankingText.text = "Erro ao carregar o ranking.";
        }
    }

    // Método para limpar o ranking
    public void ClearRanking()
    {
        // Limpa o ranking no RankingManager
        if (rankingManager != null)
        {
            rankingManager.ClearRanking();
            ShowRanking(); // Atualiza o texto do ranking
        }
    }

    // Ocultar o painel de ranking e voltar ao menu inicial
    public void ReturnToMenuFromRanking()
    {
        rankingPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    // Exibir o painel about
    public void ShowAbout()
    {
        // Exibe o painel de ranking
        aboutPanel.SetActive(true);

        // Oculta o menu inicial
        startPanel.SetActive(false);
    }

    // Ocultar o painel de about e voltar ao menu inicial
    public void ReturnToMenuFromAbout()
    {
        aboutPanel.SetActive(false);
        startPanel.SetActive(true);
    }
}
