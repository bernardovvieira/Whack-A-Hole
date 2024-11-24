# O Jogo
 
 "Whack-a-Mole" é um jogo divertido e desafiador onde você precisa agir rápido para acertar os "moles" (toupeiras) que surgem de buracos espalhados pela tela. A cada acerto, você ganha pontos, mas tome cuidado! Se errar, o tempo será reduzido e, se acertar uma bomba, será eliminado. O objetivo é acumular a maior pontuação possível antes que o tempo acabe. Enfrente diferentes níveis de dificuldade e tente superar seu próprio recorde no ranking! Prepare-se para uma experiência emocionante e prove sua habilidade em reflexos rápidos!

### Desenvolvedores

 O jogo desenvolvido pelos acadêmicos Bernardo Vivian Vieira (179835) e Yeun Haur Kang (193593), estudantes da Universidade de Passo Fundo (UPF) na disciplina Computação Gráfica e Realidade Virtual do sexto semestre de Ciência da Computação.

## Funcionalidades
- Sistema de Pontuação e Ranking: acompanhe sua pontuação em tempo real e registre seus recordes.
- Vários Níveis de Dificuldade: o jogo aumenta progressivamente o desafio.
- Feedback Visual e Sonoro: respostas claras para acertos, erros e fim de jogo.
- Gestão de Ranking: visualize, atualize e limpe o ranking de melhores jogadores.
- Interface Intuitiva: painéis de início, sobre, fim de jogo e ranking acessíveis.

## Como Jogar
1. Clique no botão Play para iniciar.
2. Acerte as toupeiras que surgirem na tela:
   - Toupeiras comuns: +1 ponto.
   - Toupeiras com capacete: requerem dois cliques para serem eliminadas.
   - Bombas: finalizam o jogo.
3. Evite errar, pois isso reduz o tempo restante.
4. Tente superar seu recorde antes que o tempo acabe!


# Documentação Técnica

## Estrutura do Projeto

### Principais Scripts
1. GameManager.cs
   - Gerencia o estado do jogo (início, fim, pontuação, tempo).
   - Controla a interface do usuário (painéis, botões e ranking).
2. Mole.cs
   - Gerencia o comportamento das toupeiras (aparecimento, tipos, resposta a cliques).
   - Define a dificuldade com base no nível.
3. RankingManager.cs
   - Gerencia o sistema de ranking (adicionar, ordenar, limpar e salvar os dados).

### Classes Principais

*GameManager*

_Responsável por:_
   - Controle do estado do jogo.
   - Interação com elementos visuais e sonoros.
   - Integração com o sistema de ranking.

_Principais métodos:_
   - StartGame(): inicializa o jogo.
   - GameOver(int type): finaliza o jogo (por tempo ou bomba).
   - AddScore(int moleIndex): incrementa a pontuação.
   - RestartGame(): reinicia o jogo.
   - ShowRanking(): exibe o painel de ranking.

*Mole*

_Reponsável por:_
   - Controla as toupeiras, seu tipo (comum, capacete, bomba) e interações.

_Principais métodos:_
   - Activate(int level): ativa uma toupeira com base na dificuldade.
   - OnMouseDown(): responde ao clique do jogador.
   - Hide(): oculta a toupeira.

*RankingManager*

_Responsável por:_
   - Gerencia o ranking, ordenando e salvando as melhores pontuações.

_Principais métodos:_
   - AddOrUpdateScore(string playerName, int score): adiciona ou atualiza uma pontuação.
   - LoadRanking(): carrega o ranking salvo.
   - ClearRanking(): limpa os dados de ranking.
  
## Como Executar o Projeto

1. Pré-requisitos:
   - Unity 2021.3 ou superior.
2. Passos:
   - Clone o repositório.
   - Abra o projeto no Unity.
   - Execute a cena principal (MainScene).
3. Controles:
   - Use o mouse para interagir com as toupeiras e botões.

## Tecnologias Utilizadas
- Engine: Unity
- Linguagem: C# (MonoBehaviour e UnityEngine)
- Gerenciamento de Dados: PlayerPrefs para armazenamento local.
