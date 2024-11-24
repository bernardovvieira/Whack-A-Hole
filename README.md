# O Jogo
 
 "Whack-a-Mole" é um jogo divertido e desafiador onde você precisa agir rápido para acertar os "moles" (toupeiras) que surgem de buracos espalhados pela tela. A cada acerto, você ganha pontos, mas tome cuidado! Se errar, o tempo será reduzido e, se acertar uma bomba, será eliminado. O objetivo é acumular a maior pontuação possível antes que o tempo acabe. Enfrente diferentes níveis de dificuldade e tente superar seu próprio recorde no ranking! Prepare-se para uma experiência emocionante e prove sua habilidade em reflexos rápidos!

## Funcionalidades
- Sistema de Pontuação e Ranking: acompanhe sua pontuação em tempo real e registre seus recordes.
- Vários Níveis de Dificuldade: o jogo aumenta progressivamente o desafio.
- Feedback Visual e Sonoro: respostas claras para acertos, erros e fim de jogo.
- Gestão de Ranking: visualize, atualize e limpe o ranking de melhores jogadores.
- Interface Intuitiva: painéis de início, sobre, fim de jogo e ranking acessíveis.

## Como Jogar
1. Clique no botão Play para iniciar.
2. Acerte as toupeiras que surgirem na tela:
_ - Toupeiras comuns: +1 ponto.
_ - Toupeiras com capacete: requerem dois cliques para serem eliminadas.
_ - Bombas: finalizam o jogo.
3. Evite errar, pois isso reduz o tempo restante.
4. Tente superar seu recorde antes que o tempo acabe!


# Documentação Técnica

## Estrutura do Projeto

### Principais Scripts
1. GameManager.cs
_ - Gerencia o estado do jogo (início, fim, pontuação, tempo).
_ - Controla a interface do usuário (painéis, botões e ranking).
2. Mole.cs
_ - Gerencia o comportamento das toupeiras (aparecimento, tipos, resposta a cliques).
_ - Define a dificuldade com base no nível.
3. RankingManager.cs
_ - Gerencia o sistema de ranking (adicionar, ordenar, limpar e salvar os dados).

### Classes Principais

*GameManager*

_Responsável por:_
_ -Controle do estado do jogo.
_ -Interação com elementos visuais e sonoros.
_ -Integração com o sistema de ranking.

_Principais métodos:_
_ - StartGame(): inicializa o jogo.
_ - GameOver(int type): finaliza o jogo (por tempo ou bomba).
_ - AddScore(int moleIndex): incrementa a pontuação.
_ - RestartGame(): reinicia o jogo.
_ - ShowRanking(): exibe o painel de ranking.

*Mole*

_Reponsável por:_
_ - Controla as toupeiras, seu tipo (comum, capacete, bomba) e interações.

_Principais métodos:_
_ - Activate(int level): ativa uma toupeira com base na dificuldade.
_ - OnMouseDown(): responde ao clique do jogador.
_ - Hide(): oculta a toupeira.

*RankingManager*

_Responsável por:_
_ - Gerencia o ranking, ordenando e salvando as melhores pontuações.

_Principais métodos:_
_ - AddOrUpdateScore(string playerName, int score): adiciona ou atualiza uma pontuação.
_ - LoadRanking(): carrega o ranking salvo.
_ - ClearRanking(): limpa os dados de ranking.
  
## Como Executar o Projeto

1. Pré-requisitos:
_ - Unity 2021.3 ou superior.
2. Passos:
_ - Clone o repositório.
_ - Abra o projeto no Unity.
_ - Execute a cena principal (MainScene).
3. Controles:
_ - Use o mouse para interagir com as toupeiras e botões.

## Tecnologias Utilizadas
- Engine: Unity
- Linguagem: C# (MonoBehaviour e UnityEngine)
- Gerenciamento de Dados: PlayerPrefs para armazenamento local.
