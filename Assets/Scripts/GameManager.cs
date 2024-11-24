using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Timeline.Actions;

/// Gerenciador principal do jogo, controla o estado geral, interface do usu�rio e l�gica de jogo.
public class GameManager : MonoBehaviour
{
    // Lista de objetos Mole no jogo
    [SerializeField] private List<Mole> moles;

    [Header("Objetos de UI")]
    [SerializeField] private GameObject playButton;      // Bot�o de iniciar o jogo
    [SerializeField] private GameObject gameUI;          // Interface de jogo
    [SerializeField] private GameObject startPanel;      // Painel de in�cio
    [SerializeField] private TMPro.TextMeshProUGUI timeText;  // Texto do tempo restante
    [SerializeField] private TMPro.TextMeshProUGUI scoreText; // Texto da pontua��o
    [SerializeField] private GameObject endBombPanel;     // Painel de fim de jogo (bomba explodiu)
    [SerializeField] private GameObject endTimePanel;     // Painel de fim de jogo (tempo acabou)
    [SerializeField] private GameObject restartButton;    // Bot�o de reiniciar o jogo
    [SerializeField] private GameObject returnToMenuButton; // Bot�o de voltar ao menu
    [SerializeField] private TextMeshProUGUI finalScoreText; // Texto da pontua��o final

    [Header("Ranking UI")]
    [SerializeField] private RankingManager rankingManager; // Refer�ncia ao RankingManager
    [SerializeField] private TMP_InputField playerNameInput; // Campo para o nome do jogador
    private string playerName = "Jogador"; // Nome padr�o do jogador
    [SerializeField] private GameObject rankingPanel; // Painel do ranking
    [SerializeField] private TextMeshProUGUI rankingText; // Campo de texto do ranking

    [Header("Sobre UI")]
    [SerializeField] private GameObject aboutPanel; // Painel de informa��es sobre o jogo

    [Header("Sons")]
    [SerializeField] private AudioClip gameMenuSound; // Som de fundo na tela de menu
    [SerializeField] private AudioClip buttonClickSound; // Som ao apertar os botões
    [SerializeField] private AudioClip gameOverSound; // Som de fim de jogo
    [SerializeField] private AudioClip gameOverSound2; // Som de fim de jogo 2
    [SerializeField] private AudioClip backgroundSound; // Som de fundo
    [SerializeField] private AudioClip bonkHitSound; // Som ao acertar a toupeira
    [SerializeField] private AudioClip bombExplosion; // Som de explosão da bomba
    [SerializeField] private AudioSource audioSource; // Fonte de �udio para tocar os sons

    // Vari�veis configur�veis (ajust�veis no c�digo)
    private float startingTime = 30f; // Tempo inicial do jogo

    // Vari�veis globais
    private float timeRemaining; // Tempo restante
    private HashSet<Mole> currentMoles = new HashSet<Mole>(); // Moles atualmente ativos
    private int score; // Pontua��o do jogador
    private bool playing = false; // Indica se o jogo est� em andamento

    // Ocultar endBombPanel e endTimePanel no in�cio
    void Start()
    {
        endBombPanel.SetActive(false);
        endTimePanel.SetActive(false);
        rankingPanel.SetActive(false);
        aboutPanel.SetActive(false);

        // Obt�m o componente AudioSource do GameObject se n�o tiver sido atribu�do
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.PlayOneShot(gameMenuSound);
    }


    /// Som ao apertar os botões.
    public void ButtonSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// M�todo p�blico para iniciar o jogo, chamado pelo bot�o de "Play".
    public void StartGame()
    {
        // Captura o nome do jogador ou usa um nome padr�o
        playerName = string.IsNullOrWhiteSpace(playerNameInput.text) ? "Jogador" : playerNameInput.text.Trim();

        // Ocultar o painel de in�cio
        startPanel.SetActive(false);

        // Configurar interface do usu�rio
        playButton.SetActive(false);
        gameUI.SetActive(true);

        audioSource.Stop();
        // Iniciar o som de fundo
        audioSource.PlayOneShot(backgroundSound);

        // Ocultar todos os moles e configurar seus �ndices
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

    /// Efeito sonoro ao acertar a toupeira.
    public void PlayBonkSound()
    {
        if (audioSource != null && bonkHitSound != null)
        {
            audioSource.PlayOneShot(bonkHitSound);
        }
    }

    /// Finaliza o jogo, exibindo a mensagem apropriada.
    public void GameOver(int type)
    {
        // Parar o som de fundo
        audioSource.Stop();

        // Toca som de explosão da bomba
        audioSource.PlayOneShot(bombExplosion);

        // Toca o som de fim de jogo
        audioSource.PlayOneShot(gameOverSound);

        // Toca o segundo som de fim de jogo
        audioSource.PlayOneShot(gameOverSound2);

        if (type == 0)
        {
            endTimePanel.SetActive(true);  // Exibe o painel de fim de jogo por tempo
            finalScoreText.text = $"Sua pontua��o final: {score} pontos"; // Mostra a pontua��o final
        }
        else
        {
            endBombPanel.SetActive(true);  // Exibe o painel de fim de jogo por bomba
            finalScoreText.text = $"Sua pontua��o final: {score} pontos"; // Mostra a pontua��o final
        }


        // Salvar a pontua��o no ranking
        SaveScore();

        // Ativar o bot�o de reiniciar e o bot�o de voltar ao menu
        restartButton.SetActive(true);  // Torna o bot�o de reiniciar vis�vel
        returnToMenuButton.SetActive(true); // Torna o bot�o de voltar ao menu vis�vel

        // Parar todos os moles
        foreach (Mole mole in moles)
        {
            mole.StopGame();
        }

        // Finalizar o jogo e mostrar o bot�o de rein�cio
        playing = false;
        playButton.SetActive(true);
    }

    /// Salva a pontua��o do jogador no sistema de ranking.
    private void SaveScore()
    {
        if (rankingManager != null)
        {
            rankingManager.AddOrUpdateScore(playerName, score);  // Usando o novo m�todo
        }
        else
        {
            Debug.LogWarning("RankingManager n�o est� atribu�do!");
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

            // Verificar se � necess�rio ativar mais moles
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


    /// Incrementa a pontua��o e atualiza o estado do jogo ao acertar um mole.
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

    /// Reinicia o jogo (bot�o de reiniciar)
    public void RestartGame()
    {
        // Ocultar os pain�is de fim de jogo, o bot�o de reiniciar e o bot�o de voltar ao menu
        endBombPanel.SetActive(false);
        endTimePanel.SetActive(false);
        restartButton.SetActive(false);
        returnToMenuButton.SetActive(false);
        finalScoreText.text = ""; // Limpar o texto da pontua��o final

        // Iniciar o jogo novamente
        StartGame();
    }

    /// Volta para o menu principal e encerra o jogo atual (bot�o "Voltar ao Menu")
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

        // Exibir o painel de in�cio (menu principal)
        startPanel.SetActive(true);

        // Esconder o painel de fim de jogo, bot�o de reiniciar e bot�o de voltar ao menu
        endBombPanel.SetActive(false);
        endTimePanel.SetActive(false);
        restartButton.SetActive(false);
        returnToMenuButton.SetActive(false);
        finalScoreText.text = ""; // Limpar o texto da pontua��o final

        // Exibir o bot�o "Play" para reiniciar o jogo
        playButton.SetActive(true);
    }

    // Exibir o painel de ranking
    public void ShowRanking()
    {
        // Limpa o texto de ranking antes de exibir novamente
        rankingText.text = "";

        // Atualiza o texto do ranking com as pontua��es
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

    // M�todo para limpar o ranking
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
