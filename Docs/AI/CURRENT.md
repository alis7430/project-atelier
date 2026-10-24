# Project Atelier — Current Development Context

> 마지막 갱신: 2026-10-24.
> 목표는 개발자 확인, 구현은 저장된 소스의 정적 확인을 근거로 한다.
> 소스 기준 커밋: 81014b0. Unity 실행·빌드·테스트 결과는 미확인이다.

## 1. 현재 우선 작업

**Slay the Spire 2를 레퍼런스로 하는 맵 화면을 개발하고
MapTestScene에서 Unity Editor 동작을 확인한다.**

- [개발자 확인] 생성 알고리즘의 기본 틀은 마련되어 있다.
- [소스 확인] MapView의 레이아웃, 노드·선 생성과 상태 표시가 구현되어 있다.
- [소스·자산 확인] MapTestScene의 생성·표시·초기 상태·클릭·방문 처리와
  저장된 씬의 Config/Manager/View 및 노드·선 프리팹 참조가 연결되어 있다.
- [이력] 2026-10-22 기준 문서 작성 이후 20b475d에서 테스트 연결이 진행되었다.
  당시의 씬 미연결 기록을 현재 상태로 사용하지 않는다.
- [다음 확인] 실제 화면·입력·재표시와 생성 규칙. 현재 저장소 근거만으로
  개발자의 Editor 검증 완료 여부는 확정하지 않는다.

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
| 맵 생성 | [기본 틀] 레이어·경로·좌표 변동·교차 제거 | Assets/_Projects/Scripts/Map/MapGenerator.cs |
| 맵 표시 | [구현 확인] 레이아웃, 노드 상태, 방문·이동 가능 간선 표시 | Assets/_Projects/Scripts/Map/MapView.cs |
| 방문 진행 | [코드 확인] CanVisit/TryVisitNode 및 방문 이력 | Assets/_Projects/Scripts/Map/MapManager.cs |
| 테스트 연결 | [소스·자산 확인] 생성·표시·입력·정리 및 씬/프리팹 참조 연결 | Assets/_Projects/Scripts/Map/Test/MapTestScene.cs, Assets/_Projects/Scenes/MapTestScene.unity |
| 작성 자산 | [정적 확인] 현재 필드명과 일치, 마지막 레이어 BossEnemy 설정 | Assets/Resources/ScriptableObject/Map/MapConfig.asset |
| 런 데이터 | [부분 코드] RuntimeManager 존재, 등록·사용 연결 없음 | Assets/_Projects/Scripts/System/RuntimeManager.cs |
| RunManager | [계획] 해당 시스템 구현 발견 못함 | 개발자 요구 |
| 맵 풀링 | [계획] MapView는 현재 Instantiate/Destroy 사용 | Assets/_Projects/Scripts/Map/MapView.cs |
| 저장·복원 | [미구현 관찰] 영속 저장 구현 발견 못함, 요구 범위 미정 | 검토한 게임 및 프레임워크 코드 |

씬 전환 유지와 게임 종료 후 디스크 복원은 다른 요구사항이다.
후자는 현재 필수 작업으로 확정하지 않았다.

## 4. 관련 코드와 통합 시 확인할 점

- 테스트 진입 경로는 MapTestScene.Start의 MapGenerator 생성이다.
  SystemRng에 씬의 _seed를 전달하며 저장된 값은 99999다.
  MapManager.GenerateMap의 고정 시드 12345 경로는 남아 있지만 Start에서 호출하지 않는다.
- SetMap은 데이터와 방문 상태만 설정한다. MapTestScene이 이어서
  ShowMap과 최초 RefreshStates를 호출한다.
- MapTestScene은 OnEnable/OnDisable에서 MapView.OnNodeClicked를 구독·해제하고,
  HandleNodeClicked에서 MapNode.Node.Id를 TryVisitNode에 전달한다.
  MapManager의 private OnNodeClicked(int)는 이 경로에서 사용하지 않는다.
- MapTestScene.Clear는 View 이벤트를 해제하고 MapView.Clear를 호출한다.
- 저장된 씬의 MapManager._view와 MapTestScene._mapView는 같은 MapView를 참조한다.
  노드·선 프리팹은 Assets/Resources/Prefabs/UI/Map/에 있으며 GUID와 fileID를 대조했다.
- MapConfig.asset은 numOfPreBossNodes/numOfStartingNodes/NodeApartDistance 등
  현재 필드명으로 갱신되었다. 마지막 레이어는 BossEnemy, randomNodeValue=0이다.
  값은 여전히 테스트용이며 다른 요구사항을 추론하는 근거로 삼지 않는다.
- 생성기는 마지막 레이어를 종착점으로 선택하지만 BossEnemy를 일반 규칙으로 강제하지 않는다.
- 교차 연결 제거 후 종착점 도달 가능성 검증·보정이 없다.
  확정된 규칙에 맞는지 개발자가 검증해야 한다.
- SceneManagerEX.LoadScene은 Managers.Clear를 호출한다.
  RuntimeManager를 그대로 등록하면 Clear가 Seed/Random을 지우므로
  향후 RunManager의 런 수명과 씬 정리 정책을 구분해야 한다.
- 프레임워크 명칭은 URDev.GameFramework로 확정되었으며, Framework.cs와 asmdef에서 일치한다. 명칭 이전을 미완료 작업으로 취급하지 않는다.

## 5. 다음 작업 순서

다음은 현재 목표에 맞춘 제안이며 별도 시스템 구현 승인이 아니다.

1. 연결된 MapTestScene에서 화면·입력·노드 및 간선 상태를 개발자가 확인한다.
2. 재생성·재표시·정리 시 이벤트와 표시 객체의 수명을 확인한다.
3. 여러 설정·시드에서 Boss 종착점, 모든 시작점의 도달 가능성,
   막다른 길 금지 검증.
4. 테스트 결과에 따라 설정·표현을 조정하고 연결 책임 이전 시점·위치를 결정한다.
5. RunManager와 맵 풀링의 구체 수명·소유권 설계.

연결 책임 이전, RunManager, 풀링을 현재 화면 확인의 선행조건으로
불필요하게 확대하지 않는다.

## 6. 개발자 검증 체크리스트

아직 실행한 결과가 아니다. 저장된 참조 연결을 확인한 것과 런타임 유효성은 구분한다.

- [ ] 테스트 설정과 노드·선 참조가 유효하다.
- [ ] 생성된 노드와 연결선이 동일 좌표계에 표시된다.
- [ ] 첫 선택 가능 노드와 방문 후 다음 노드가 올바르게 표시된다.
- [ ] 잠긴 노드 선택이 거부된다.
- [ ] 방문한 간선과 현재 노드에서 이동 가능한 간선의 색상이 올바르다.
- [ ] 마지막 노드가 BossEnemy다.
- [ ] 모든 시작점에서 Boss 종착점에 도달한다.
- [ ] 종착점 외의 진행 가능한 경로에 막다른 길이 없다.
- [ ] 재생성·재표시 과정에 중복 이벤트나 남은 표시 객체가 없다.
- [ ] 향후 런 통합 시 씬 전환 후 시드·진행이 유지된다.
- [ ] 향후 풀링 통합 시 현재 지도를 재열어도 진행 상태가 유지된다.

## 7. 변경 및 협업 상태

- 2026-10-22 기준 분석 당시의 미커밋 상태는 과거 관찰이며 현재 상태가 아니다.
- 20b475d에서 MapView·테스트 씬 연결과 MapConfig 정합성이 갱신되었다.
- 81014b0에서 연결된 노드만 반환하는 필터가 복원되고 노드 아이콘 크기가 조정되었다.
- AGENTS.md와 Docs/AI 문서는 실제로 존재한다.
- 2026-10-24 개발자가 컨텍스트 복원 보고 후 확인된 진행 상황의 문서 반영을 요청했다.
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
