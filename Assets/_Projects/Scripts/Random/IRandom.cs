/// <summary>
/// 난수 생성기 인터페이스
/// 시드는 IRng를 구현한 클래스에서 생성자에 전달하여 초기화한다.
/// RNG를 교체할 때 IRandom 인터페이스를 구현한 다른 RNG 클래스를 만들어서 교체하면 된다.
/// </summary>
public interface IRandom
{
    int Range(int min, int max); // min ~ max 사이의 난수 반환
    float Value();               // 0.0 ~ 1.0 사이의 난수 반환
}
