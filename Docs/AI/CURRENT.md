# Project Atelier — Current Development Context

> 마지막 갱신: 2026-10-26.
> 목표는 개발자 확인, 구현은 저장된 소스의 정적 확인을 근거로 한다.
> 2026-10-26 작업 트리의 관련 소스를 정적으로 확인했다(조회 HEAD: 2c9c332).
> 재표시·새 맵 생성·비활성화/재활성화·정리 후 재표시는 개발자가 테스트 후 문제없음을 보고했다.
> AI가 Unity·빌드·테스트를 실행한 것은 아니며 상세 로그·반복 횟수는 제공되지 않았다.

## 1. 현재 우선 작업

**Slay the Spire 2를 레퍼런스로 하는 맵 화면을 개발하고
MapTestScene에서 Unity Editor 동작을 확인한다.**

- [개발자 확인] 생성 알고리즘의 기본 틀은 마련되어 있다.
- [소스 확인] MapView의 레이아웃, 노드·선 생성과 상태 표시가 구현되어 있다.
- [소스·자산 확인] MapTestScene의 생성·표시·초기 상태·클릭·방문 처리와
  저장된 씬의 Config/Manager/View 및 노드·선 프리팹 참조가 연결되어 있다.
- [이력] 2026-10-22 기준 문서 작성 이후 20b475d에서 테스트 연결이 진행되었다.
  당시의 씬 미연결 기록을 현재 상태로 사용하지 않는다.
- [완료 — 개발자 보고, 2026-10-26] 맵 재표시·새 맵 생성·화면 비활성화/재활성화·
  정리 후 재표시를 위한 코드를 적용하고 테스트 결과 문제없음을 확인했다.
- [다음 확인] 여러 시드·설정에서 생성 규칙과 실패 처리를 확인하고,
  이어서 씬 전환 시 런 상태 보존의 소유권·수명을 설계한다.

맵 구현 참고:
https://github.com/alis7430/slay-the-spire-map-in-unity

## 2. 기존에 확정된 요구사항

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
| 맵 생성 | [코드 확인] 생성·입력 검증·마지막 레이어 Boss 강제·교차 제거 후 경로 검증 | Assets/_Projects/Scripts/Map/MapGenerator.cs |
| 맵 표시 | [구현 확인] 레이아웃, 노드 상태, 방문·이동 가능 간선 표시 | Assets/_Projects/Scripts/Map/MapView.cs |
| 방문 진행 | [코드 확인] CanVisit/TryVisitNode 및 방문 이력 | Assets/_Projects/Scripts/Map/MapManager.cs |
| 테스트 연결 | [소스·자산 확인] 생성·표시·입력·정리 및 씬/프리팹 참조 연결 | Assets/_Projects/Scripts/Map/Test/MapTestScene.cs, Assets/_Projects/Scenes/MapTestScene.unity |
| 재표시·정리 안정화 | [소스 확인·개발자 테스트 보고] 생성/재표시/표시 전용 정리 진입점 분리, 테스트 문제없음 | MapTestScene.GenerateNewMap/RedisplayCurrentMap/ClearDisplayedMap |
| 작성 자산 | [정적 확인] 현재 필드명과 일치, 마지막 레이어 BossEnemy 설정 | Assets/Resources/ScriptableObject/Map/MapConfig.asset |
| 런 데이터 | [부분 코드] RuntimeManager 존재, 등록·사용 연결 없음 | Assets/_Projects/Scripts/System/RuntimeManager.cs |
| RunManager | [계획] 해당 시스템 구현 발견 못함 | 개발자 요구 |
| 맵 풀링 | [계획] MapView는 현재 Instantiate/Destroy 사용 | Assets/_Projects/Scripts/Map/MapView.cs |
| 저장·복원 | [미구현 관찰] 영속 저장 구현 발견 못함, 요구 범위 미정 | 검토한 게임 및 프레임워크 코드 |

씬 전환 유지와 게임 종료 후 디스크 복원은 다른 요구사항이다.
후자는 현재 필수 작업으로 확정하지 않았다.

## 4. 관련 코드와 통합 시 확인할 점

- 테스트 진입은 MapTestScene.Start → GenerateNewMap이다.
  SystemRng에 씬의 _seed를 전달하며 현재 저장된 값은 12312다.
  MapManager.GenerateMap의 고정 시드 12345 경로는 남아 있지만 Start에서 호출하지 않는다.
- GenerateNewMap은 생성 성공 후에만 SetMap으로 맵·진행을 초기화하고 RedisplayCurrentMap을 호출한다.
  생성 결과가 null이면 기존 맵·진행을 유지한다.
- RedisplayCurrentMap은 CurrentMap으로 ShowMap → RefreshStates를 호출한다.
  SetMap을 호출하지 않으므로 같은 맵의 진행을 유지한다.
- ClearDisplayedMap은 MapView.Clear만 호출하여 표시를 비우고 클릭 구독과 진행은 유지한다.
  세 확인용 ContextMenu 메서드는 Play Mode에서만 동작한다.
- MapTestScene은 OnEnable/OnDisable에서 MapView.OnNodeClicked를 구독·해제하고,
  HandleNodeClicked에서 MapNode.Node.Id를 TryVisitNode에 전달한다.
  MapManager의 private OnNodeClicked(int)는 이 경로에서 사용하지 않는다.
- MapTestScene.Clear는 씬 정리 경로로 View 이벤트를 해제하고 MapView.Clear를 호출한다.
  화면 정리 후 재표시 확인에는 ClearDisplayedMap을 사용한다.
- 저장된 씬의 MapManager._view와 MapTestScene._mapView는 같은 MapView를 참조한다.
  노드·선 프리팹은 Assets/Resources/Prefabs/UI/Map/에 있으며 GUID와 fileID를 대조했다.
- MapConfig.asset은 numOfPreBossNodes/numOfStartingNodes/NodeApartDistance 등
  현재 필드명으로 갱신되었다. 마지막 레이어는 BossEnemy, randomNodeValue=0이다.
  값은 여전히 테스트용이며 다른 요구사항을 추론하는 근거로 삼지 않는다.
- 생성기는 마지막 레이어의 BossEnemy 타입을 강제하고 입력 설정을 검증한다.
  교차 제거 후 ValidatePaths로 선택한 시작점/보스 직전 노드의 유지, 최종 Boss,
  막다른 길, 레이어 방향, 전후 연결 정합성 등을 확인하고 실패 시 null을 반환한다.
  별도의 경로 복구·자동 재시도는 없으며 여러 시드·설정의 실행 결과는 이번 보고로 확정하지 않는다.
- SceneManagerEX.LoadScene은 Managers.Clear를 호출한다.
  RuntimeManager를 그대로 등록하면 Clear가 Seed/Random을 지우므로
  향후 RunManager의 런 수명과 씬 정리 정책을 구분해야 한다.
- 프레임워크 명칭은 URDev.GameFramework로 확정되었으며, Framework.cs와 asmdef에서 일치한다. 명칭 이전을 미완료 작업으로 취급하지 않는다.

## 5. 다음 작업 순서

다음은 현재 목표에 맞춘 제안이며 별도 시스템 구현 승인이 아니다.

1. 여러 시드·설정에서 생성 규칙과 실패 시 기존 맵·진행 유지 확인.
   MapManager.ValidateMapSeeds는 10,000회 생성의 예외/null 여부를 확인하는 기존 진입점이다.
   수행 여부·결과는 미확인이고, 설정별 범위와 실패 시드를 기록한다.
2. RunManager의 시드·맵·진행 소유권, RuntimeManager와의 관계,
   씬 정리와 런 종료의 경계를 설계한다.
3. 씬 전환 후 현재 맵·진행을 복원하여 표시하고, 새 런에서만 초기화되는지 확인한다.
4. 풀링 단위·대여/반환 시 상태와 이벤트 정책을 정하고 통합한다.
5. 확인 결과에 따라 설정·표현 및 MapTestScene의 연결 책임 이전 위치를 결정한다.

연결 책임 이전, RunManager, 풀링을 현재 화면 확인의 선행조건으로
불필요하게 확대하지 않는다.

## 6. 개발자 검증 체크리스트

체크는 개발자 보고를 근거로 한다. 이번 작업 범위는 문제없음으로 보고되었으나
세부 항목별 로그·계측 결과는 제공되지 않았다. 나머지 미확인 항목을 통과로 추정하지 않는다.

- [ ] 테스트 설정과 노드·선 참조가 유효하다.
- [ ] 생성된 노드와 연결선이 동일 좌표계에 표시된다.
- [ ] 첫 선택 가능 노드와 방문 후 다음 노드가 올바르게 표시된다.
- [ ] 잠긴 노드 선택이 거부된다.
- [ ] 방문한 간선과 현재 노드에서 이동 가능한 간선의 색상이 올바르다.
- [ ] 마지막 노드가 BossEnemy다.
- [ ] 모든 시작점에서 Boss 종착점에 도달한다.
- [ ] 종착점 외의 진행 가능한 경로에 막다른 길이 없다.
- [x] 재표시·새 맵 생성·비활성화/재활성화·정리 후 재표시 안정화: 개발자 테스트 결과 문제없음(2026-10-26).
- [ ] 여러 시드·설정의 생성 및 실패 시 기존 맵·진행 유지 결과 기록.
- [ ] 향후 런 통합 시 씬 전환 후 시드·진행이 유지된다.
- [ ] 향후 풀링 통합 시 현재 지도를 재열어도 진행 상태가 유지된다.

## 7. 변경 및 협업 상태

- 2026-10-22 기준 분석 당시의 미커밋 상태는 과거 관찰이며 현재 상태가 아니다.
- 20b475d에서 MapView·테스트 씬 연결과 MapConfig 정합성이 갱신되었다.
- 81014b0에서 연결된 노드만 반환하는 필터가 복원되고 노드 아이콘 크기가 조정되었다.
- AGENTS.md와 Docs/AI 문서는 실제로 존재한다.
- 2026-10-24 개발자가 컨텍스트 복원 보고 후 확인된 진행 상황의 문서 반영을 요청했다.
- 2026-10-26 개발자가 재표시·정리 안정화 제안을 수용하여 코드를 수정하고 테스트 문제없음을 보고했다.
  이번 명시 요청으로 Docs/AI에 구현 상태·보고된 결과·후속 작업을 반영했다.
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
