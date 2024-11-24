using System.Collections;
using UnityEngine;

public class Mole : MonoBehaviour {
    [Header("Gráficos")]
    [SerializeField] private Sprite mole;              // Sprite do mole padrão
    [SerializeField] private Sprite moleHardHat;       // Sprite do mole com capacete
    [SerializeField] private Sprite moleHatBroken;     // Sprite do capacete quebrado
    [SerializeField] private Sprite moleHit;           // Sprite do mole acertado
    [SerializeField] private Sprite moleHatHit;        // Sprite do capacete ao ser atingido

    [Header("Gerenciador do Jogo")]
    [SerializeField] private GameManager gameManager;  // Referência ao GameManager

    // Posição inicial e final do mole (usadas para animar o surgimento e ocultação)
    private Vector2 startPosition = new Vector2(0f, -2.56f);
    private Vector2 endPosition = Vector2.zero;

    // Duração das animações e tempo de exibição do mole
    private float showDuration = 0.5f;
    private float duration = 1f;

    // Componentes do mole
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider2D;

    // Parâmetros do colisor
    private Vector2 boxOffset;
    private Vector2 boxSize;
    private Vector2 boxOffsetHidden;
    private Vector2 boxSizeHidden;

    // Parâmetros do mole
    private bool hittable = true; // Define se o mole pode ser acertado
    public enum MoleType { Standard, HardHat, Bomb }; // Tipos de mole
    private MoleType moleType;  // Tipo atual do mole
    private float hardRate = 0.25f; // Probabilidade de mole com capacete
    private float bombRate = 0f;    // Probabilidade de mole bomba
    private int lives;              // Vidas restantes do mole
    private int moleIndex = 0;      // Índice único do mole

    // Corrotina para animar o aparecimento e desaparecimento do mole
    private IEnumerator ShowHide(Vector2 start, Vector2 end) {
        transform.localPosition = start;

        float elapsed = 0f;
        while (elapsed < showDuration) {
            transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = end;
        boxCollider2D.offset = boxOffset;
        boxCollider2D.size = boxSize;

        yield return new WaitForSeconds(duration);

        elapsed = 0f;
        while (elapsed < showDuration) {
            transform.localPosition = Vector2.Lerp(end, start, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = start;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;

        if (hittable) {
            hittable = false;
            gameManager.Missed(moleIndex, moleType != MoleType.Bomb);
        }
    }

    // Oculta o mole
    public void Hide() {
        transform.localPosition = startPosition;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;
    }

    // Corrotina para ocultar rapidamente o mole
    private IEnumerator QuickHide() {
        yield return new WaitForSeconds(0.25f);
        if (!hittable) {
            Hide();
        }
    }

    // Resposta ao clique do jogador
    private void OnMouseDown()
    {
        if (hittable)
        {

            switch (moleType)
            {
                case MoleType.Standard:
                    spriteRenderer.sprite = moleHit;
                    gameManager.AddScore(moleIndex);
                    StopAllCoroutines();
                    StartCoroutine(QuickHide());
                    hittable = false;
                    break;

                case MoleType.HardHat:
                    if (lives == 2)
                    {
                        spriteRenderer.sprite = moleHatBroken;
                        lives--;
                    }
                    else
                    {
                        spriteRenderer.sprite = moleHatHit;
                        gameManager.AddScore(moleIndex);
                        StopAllCoroutines();
                        StartCoroutine(QuickHide());
                        hittable = false;
                    }
                    break;

                case MoleType.Bomb:
                    gameManager.GameOver(1);  // Acionando o fim de jogo se for uma bomba
                    break;
            }
        }
    }


    // Cria um novo mole baseado na dificuldade atual
    private void CreateNext() {
        float random = Random.Range(0f, 1f);
        if (random < bombRate) {
            moleType = MoleType.Bomb;
            animator.enabled = true;
        } else {
            animator.enabled = false;
            random = Random.Range(0f, 1f);
            if (random < hardRate) {
                moleType = MoleType.HardHat;
                spriteRenderer.sprite = moleHardHat;
                lives = 2;
            } else {
                moleType = MoleType.Standard;
                spriteRenderer.sprite = mole;
                lives = 1;
            }
        }
        hittable = true;
    }

    // Ajusta o nível do mole (dificuldade)
    private void SetLevel(int level) {
        bombRate = Mathf.Min(level * 0.025f, 0.25f);
        hardRate = Mathf.Min(level * 0.025f, 1f);
        float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.01f, 1f);
        float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.01f, 2f);
        duration = Random.Range(durationMin, durationMax);
    }

    // Configurações iniciais do mole
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxOffset = boxCollider2D.offset;
        boxSize = boxCollider2D.size;
        boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
        boxSizeHidden = new Vector2(boxSize.x, 0f);
    }

    // Ativa o mole com base no nível atual
    public void Activate(int level) {
        SetLevel(level);
        CreateNext();
        StartCoroutine(ShowHide(startPosition, endPosition));
    }

    // Define o índice único do mole
    public void SetIndex(int index) {
        moleIndex = index;
    }

    // Para o jogo, interrompendo todas as animações do mole
    public void StopGame() {
        hittable = false;
        StopAllCoroutines();
    }
}
