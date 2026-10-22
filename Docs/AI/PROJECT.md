# Project Atelier — Project Overview

> 마지막 갱신: 2026-10-22.
> 제품 목표와 개발 방향은 개발자의 명시적 설명을 근거로 한다.
> 구현 상태는 저장된 소스·설정·자산을 정적으로 확인한 결과다.
> Unity 실행·빌드·테스트·프로파일링 결과를 의미하지 않는다.

## 1. 프로젝트 정체성과 목표

- 프로젝트명: Project Atelier.
- 장르: 덱빌딩 로그라이크.
- 게임 레퍼런스: Slay the Spire 2.
- 전체 게임 플로우는 레퍼런스와 거의 같은 방향을 목표로 한다.
  세부 기능·수치·콘텐츠까지 동일하다고 가정하지 않는다.
- 개발 목표: 프로토타입 수준의 게임 개발.
- 협업 구성: 개발자가 일러스트레이터 1명과 협업한다.
- 현재 작업: 맵 생성 기본 틀을 바탕으로 MapView를 작성하고,
  MapTestScene에서 연결하여 Unity Editor에서 확인한다.
- 현재는 코드 작업 단계이며 씬에 연결하지 않은 상태다.
- 출시 플랫폼, 성능 예산, 상세 콘텐츠 범위와 일정은 미확인이다.

확인한 작업 경로:
`D:\dev\Unity\unity-projects\project-atelier`.
다른 환경에서는 현재 프로젝트 루트를 사용한다.

## 2. 레퍼런스와 근거 구분

개발자가 지정한 맵 구현 참고 저장소:
https://github.com/alis7430/slay-the-spire-map-in-unity

- 제품 레퍼런스인 Slay the Spire 2와 맵 구현 참고 저장소를 구분한다.
- 참고 저장소가 Slay the Spire 2의 실제 알고리즘이라는 뜻은 아니다.
- 이번 문서 갱신에서는 지정 URL의 본문을 가져오지 못했다.
  외부 소스의 세부 구현·버전·라이선스를 검증했다고 기록하지 않는다.
- 유사한 이름의 다른 저장소를 지정 저장소로 바꾸어 인용하지 않는다.
- 현재 프로젝트의 구현을 판단할 때는 로컬 소스가 근거다.
- Assets/_Projects/Temp/Memo.md는 개인적인 고민을 적은 참고 자료다.
  현재 요구사항이나 승인된 설계 결정의 근거로 격상하지 않는다.

## 3. 확인된 기술 스택

| 항목 | 확인 결과 | 근거 |
| --- | --- | --- |
| Unity Editor | 6000.3.6f1 | ProjectSettings/ProjectVersion.txt |
| 언어 | C# | Assets/_Projects/Scripts/ |
| 렌더링 | URP 17.3.0 | Packages/manifest.json |
| 입력 | Input System 1.18.0 | Packages/manifest.json |
| UI | uGUI 2.0.0 | Packages/manifest.json 및 MapNode.cs |
| 테스트 패키지 | Test Framework 1.6.0 | Packages/manifest.json |
| 자체 프레임워크 | com.urdev.game-framework 0.1.0 | Packages/urdev-game-framework/package.json |
| 플러그인 | DOTween 파일 존재 | Assets/Plugins/Demigiant/DOTween/ |

- 생성된 .csproj의 LangVersion은 9.0이다. 생성 파일은 보조 근거이며
  별도 언어 정책이나 컴파일 성공의 증거는 아니다.
- GraphicsSettings의 기본 파이프라인은 PC_RPAsset이다.
- QualitySettings에 PC/Mobile 프로파일이 있지만 출시 플랫폼은 미확인이다.
- activeInputHandler 값은 1이다.
- AI Navigation 2.0.9, Timeline 1.8.10, Visual Scripting 1.9.9 등도 등록되어 있다.
  패키지 등록을 해당 게임 기능의 구현으로 해석하지 않는다.
- 자체 게임 코드에서 DOTween 사용처는 발견하지 못했다.
- 프레임워크의 최소 Unity 버전 선언은 2022.3이며 실제 호환성은 미검증이다.

## 4. 소스와 자산 구성

- Assets/_Projects/Scripts/Map/: 모델, 생성기, 진행 관리, 표시, 테스트 씬 코드.
- Assets/_Projects/Scripts/Random/: IRandom, SystemRng, RandomSeedGenerator.
- Assets/_Projects/Scripts/System/RuntimeManager.cs: 현재 시드/RNG 보관 코드.
- Assets/_Projects/Scripts/Utils/: 정수 범위, 셔플, 해상도 유틸리티.
- Assets/_Projects/Scenes/MapTestScene.unity: 빌드 설정의 활성화 씬.
- Assets/Resources/ScriptableObject/Map/: 맵 설정과 노드 아이콘 작성 데이터.
- Assets/Resources/Prefabs/UI/EventSystem.prefab: EventSystem 프리팹.
- Packages/urdev-game-framework/Runtime/: 공용 게임 프레임워크.
- Packages/urdev-game-framework/Docs/: 패키지 작성 문서.

게임 코드에는 별도 asmdef가 없으며 전역 네임스페이스를 사용한다.

프레임워크의 승인된 통일 명칭은 `URDev.GameFramework`다.
2026-10-22 개발자가 이전 대소문자 표기를 정정했다.
개발자 자체 제작 프레임워크이며 현재 asmdef 이름/rootNamespace와
Framework.cs의 네임스페이스도 `URDev.GameFramework`로 확인했다.
Framework.cs는 개발자가 변경했으며, 이번 갱신에서는 관련 문서만 수정한다.
패키지 ID는 `com.urdev.game-framework`로 유지한다.

## 5. 확인된 구현과 승인된 방향

### 현재 구현

- 설정과 IRandom을 받는 레이어형 노드 그래프 생성기.
- Map/Node 런타임 모델.
- MapManager의 방문 판정, 현재 노드 및 방문 이력.
- MapView/MapNode/MapLine의 표시·상태·클릭 이벤트 코드.
- 공용 자산·풀·사운드·JSON 테이블·입력·씬·UI 서비스.
- 씬 연결 및 Editor 동작 확인은 아직 진행 전이다.

### 승인된 방향, 구현 완료 아님

- 초기 맵 연결 책임은 MapTestScene에 둔다.
  테스트가 어느 정도 진행된 뒤 다른 위치로 옮기며 목적지는 미정이다.
- 마지막 노드는 반드시 Boss Enemy다.
- 모든 시작점에서 종착점으로 가는 경로가 있어야 한다.
- 막다른 길을 허용하지 않는다.
- 시드와 맵 진행은 씬 전환 후에도 유지한다.
- 향후 RunManager로 로그라이크 게임 한 번의 런을 관리한다.
- 맵을 풀링하여 게임 진행 중 현재 지도를 다시 볼 수 있게 한다.
- MapConfig.asset의 현재 값은 임시이며 추후 테스트에서 수정한다.

## 6. AI 협업과 장기 컨텍스트

AI는 소스를 직접 참조하는 설계 파트너·코드 리뷰어다.
개발자가 모든 실제 게임 코드를 구현한다.

- 구현 예시는 채팅에서 제공하고 생산 코드·자산·설정을 수정하지 않는다.
- 관련 클래스·인터페이스·의존성·수명을 함께 검토한다.
- 과도한 추상화나 불필요한 리팩터링을 권장하지 않는다.
- 문서 갱신이 유용한 시점에는 먼저 제안한다.
- 개발자가 명시적으로 반영을 요청한 범위에서만 문서를 수정한다.
- 이번 요청은 AGENTS.md와 Docs/AI의 기준 문서 갱신을 승인한다.
  향후 자동 수정이나 생산 코드 변경에 대한 포괄 승인은 아니다.
- 기존 결정의 이유와 세션 기록을 보존한다.

문서 역할:
- PROJECT.md: 제품 목표와 기술 스택.
- ARCHITECTURE.md: 관찰된 구조 및 승인된 목표와의 차이.
- CURRENT.md: 현재 작업과 검증 상태.
- DECISIONS.md: 승인된 결정, 이유, 미결정 세부사항.
- SESSIONS.md: 논의 당시의 기록.

현재 소스·설정·자산은 구현의 근거이며, 개발자의 명시적 요구와
Accepted 결정은 의도의 근거다. 충돌 시 양쪽을 기록한다.

## 7. 기본 작업 환경

읽기 전용 검토의 시작 예시:

```powershell
codex -C "D:\dev\Unity\unity-projects\project-atelier" --sandbox read-only
```

Unity 실행·빌드·테스트는 개발자가 수행한다.
문서 반영을 명시적으로 요청받은 경우에도 환경의 쓰기 권한 절차를 따른다.
