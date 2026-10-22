# Project Atelier — Current Development Context

> 마지막 갱신: 2026-10-22.
> 목표는 개발자 확인, 구현은 저장된 소스의 정적 확인을 근거로 한다.

## 1. 현재 우선 작업

**Slay the Spire 2를 레퍼런스로 하는 맵 화면을 작성하고
MapTestScene에 연결하여 Unity Editor에서 확인한다.**

- [개발자 확인] 생성 알고리즘의 기본 틀은 마련되어 있다.
- [진행 중] Assets/_Projects/Scripts/Map/MapView.cs 작성.
- [다음 단계] MapTestScene에서 생성·표시·입력을 연결하고 Editor에서 확인.
- [개발자 확인] 현재는 코드 작업 단계이며 씬 미연결은 알려진 개발 상태다.
  이미 연결된 기능의 회귀나 긴급 결함으로 취급하지 않는다.
- [미검증] 알고리즘의 필수 규칙 충족과 실제 화면 동작.

맵 구현 참고:
https://github.com/alis7430/slay-the-spire-map-in-unity

## 2. 이번에 확정된 요구사항

- [확정] 마지막 노드는 Boss Enemy.
- [확정] 모든 시작점에서 종착점에 도달할 경로가 있어야 함.
- [확정] 막다른 길 금지.
- [확정] 현재 연결 책임은 MapTestScene.
- [계획] 테스트 진행 후 연결 책임 이전. 이전 위치는 미정.
- [확정] 시드와 맵 진행은 씬 전환 후 유지.
- [계획] RunManager가 한 번의 런을 관리.
- [계획] 맵 풀링을 통해 진행 중 현재 지도를 다시 열 수 있도록 구현.
- [확정] MapConfig.asset 값은 임시이며 테스트 단계에서 개발자가 수정.
- [확정] 개인 Memo.md는 참고 자료일 뿐 현재 계획의 권위 있는 근거가 아님.
- [확정] 자체 프레임워크 통일 명칭은 URDev.GameFramework.
- [소스 확인] 개발자가 변경한 Framework.cs의 네임스페이스와 asmdef 이름/rootNamespace가 URDev.GameFramework로 일치함.

## 3. 현재 구현 상태

| 영역 | 상태 | 근거 |
| --- | --- | --- |
| 맵 데이터 | [구현 확인] Map/Node, ID·좌표·연결 | Assets/_Projects/Scripts/Map/Map.cs, Node.cs |
| 맵 생성 | [기본 틀] 레이어·경로·좌표 변동·교차 제거 | Assets/_Projects/Scripts/Map/MapGenerator.cs |
| 맵 표시 | [작성 중] 노드·선 생성과 노드 상태 표시 | Assets/_Projects/Scripts/Map/MapView.cs |
| 방문 진행 | [코드 확인] CanVisit/TryVisitNode 및 방문 이력 | Assets/_Projects/Scripts/Map/MapManager.cs |
| 테스트 연결 | [예정] Start/Clear는 비어 있음 | Assets/_Projects/Scripts/Map/Test/MapTestScene.cs |
| 런 데이터 | [부분 코드] RuntimeManager 존재, 등록·사용 연결 없음 | Assets/_Projects/Scripts/System/RuntimeManager.cs |
| RunManager | [계획] 해당 시스템 구현 발견 못함 | 개발자 요구 |
| 맵 풀링 | [계획] MapView는 현재 Instantiate/Destroy 사용 | Assets/_Projects/Scripts/Map/MapView.cs |
| 저장·복원 | [미구현 관찰] 영속 저장 구현 발견 못함, 요구 범위 미정 | 검토한 게임 및 프레임워크 코드 |

씬 전환 유지와 게임 종료 후 디스크 복원은 다른 요구사항이다.
후자는 현재 필수 작업으로 확정하지 않았다.

## 4. 관련 코드와 통합 시 확인할 점

- MapManager.GenerateMap은 현재 고정 시드 12345를 사용한다.
- SetMap은 데이터와 방문 상태만 설정하며 ShowMap/최초 상태 갱신은 하지 않는다.
- MapView.OnNodeClicked는 MapNode를 전달한다.
- MapManager의 private OnNodeClicked는 int를 받으며 구독 연결이 없다.
- MapTestScene이 이를 연결하는 현재 구성 책임자다.
- 저장된 씬에는 MapManager/MapView/MapNode/MapLine 참조가 없다.
- 노드·선 프리팹과 Inspector 참조도 테스트 연결 시 준비해야 한다.
- MapConfig.asset의 이전 필드명과 현재 코드가 다르다.
  현재 값의 의미를 분석해 요구사항을 추론하지 말고 테스트 준비 시 정합성을 맞춘다.
- 생성기는 마지막 레이어를 종착점으로 선택하지만 BossEnemy를 강제하지 않는다.
- 교차 연결 제거 후 종착점 도달 가능성 검증·보정이 없다.
  확정된 규칙에 맞는지 개발자가 검증해야 한다.
- SceneManagerEX.LoadScene은 Managers.Clear를 호출한다.
  RuntimeManager를 그대로 등록하면 Clear가 Seed/Random을 지우므로
  향후 RunManager의 런 수명과 씬 정리 정책을 구분해야 한다.
- 프레임워크 명칭은 URDev.GameFramework로 확정되었으며, Framework.cs와 asmdef에서 일치한다. 명칭 이전을 미완료 작업으로 취급하지 않는다.

## 5. 다음 작업 순서

다음은 현재 목표에 맞춘 제안이며 별도 시스템 구현 승인이 아니다.

1. MapView의 레이아웃·노드·선 표현 코드 작성.
2. 테스트용 MapConfig 값과 직렬화 필드 정합성 준비.
3. MapTestScene에서 생성 → 표시 → 초기 상태 → 클릭 → 방문 처리 연결.
4. Unity Editor에서 화면과 입력 확인.
5. 여러 설정·시드에서 Boss 종착점, 모든 시작점의 도달 가능성,
   막다른 길 금지 검증.
6. 테스트 결과에 따라 연결 책임 이전 시점·위치 결정.
7. RunManager와 맵 풀링의 구체 수명·소유권 설계.

연결 책임 이전, RunManager, 풀링을 현재 화면 확인의 선행조건으로
불필요하게 확대하지 않는다.

## 6. 개발자 검증 체크리스트

아직 실행한 결과가 아니다.

- [ ] 테스트 설정과 노드·선 참조가 유효하다.
- [ ] 생성된 노드와 연결선이 동일 좌표계에 표시된다.
- [ ] 첫 선택 가능 노드와 방문 후 다음 노드가 올바르게 표시된다.
- [ ] 잠긴 노드 선택이 거부된다.
- [ ] 마지막 노드가 BossEnemy다.
- [ ] 모든 시작점에서 Boss 종착점에 도달한다.
- [ ] 종착점 외의 진행 가능한 경로에 막다른 길이 없다.
- [ ] 재생성·재표시 과정에 중복 이벤트나 남은 표시 객체가 없다.
- [ ] 향후 런 통합 시 씬 전환 후 시드·진행이 유지된다.
- [ ] 향후 풀링 통합 시 현재 지도를 재열어도 진행 상태가 유지된다.

## 7. 변경 및 협업 상태

- 기준 분석 당시 MapView.cs에 기존 미커밋 줄바꿈 변경이 있었다.
  개발 목표는 Git diff가 아니라 개발자 설명으로 확인했다.
- AGENTS.md와 Docs/AI 문서는 실제로 존재한다.
- 이번 문서 갱신은 개발자가 명시적으로 반영을 요청했다.
- 향후 AI Docs 갱신이 유용하면 먼저 제안하고 승인된 범위만 반영한다.
- AI는 생산 코드·씬·자산·설정을 수정하거나 Unity/테스트를 실행하지 않는다.

## 8. 아직 정하지 않은 세부사항

- MapTestScene 이후 연결 책임의 최종 위치.
- RunManager의 구체 API, 소유 객체, 종료·재시작 정책.
- RuntimeManager를 대체할지, 통합할지, 일부 유지할지.
- 풀링 단위와 씬 간 표시 객체의 수명.
- 디스크 저장·복원의 필요 범위와 시점.
- 출시 플랫폼, 성능 목표, 상세 프로토타입 범위.

## 9. 다음 논의 템플릿

```markdown
### Task
[기능 또는 시스템]

### Goal
[기대 동작과 완료 기준]

### Current source
[검증한 경로와 심볼]

### Constraints
[호환성, 성능, 수명, 직렬화]

### Questions / unresolved points
[개발자 판단이 필요한 세부사항]

### Result
[승인된 방향, 구현 여부, 검증 상태]
```

기능 조언 전 최신 관련 소스를 다시 확인한다.
