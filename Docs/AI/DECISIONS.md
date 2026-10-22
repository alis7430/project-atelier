# Project Atelier — Architecture Decision Log

> **Purpose:** Record **accepted** decisions and the reasons behind them. Proposals should remain in chat or be marked `Proposed` until the developer explicitly accepts them.

## Decision status vocabulary

- `Accepted`: Explicitly chosen by the developer.
- `Proposed`: Candidate under discussion; not binding.
- `Superseded`: Replaced by a newer accepted decision.
- `Rejected`: Discussed and intentionally not chosen.
- `Observed`: Verified implementation; not a developer-approved architectural choice.

Acceptance records intent, not implementation completion. Track implementation and verification separately.

---

## AI-001 — Use AI as a read-only advisor

- **Date:** 2026-10-22
- **Status:** Accepted (AI collaboration policy)
- **Context:** The developer uses GPT for architecture and coding assistance and wants direct project-context access without allowing AI to edit project files.
- **Original decision:** AI tools may read the project and propose architectural improvements, code examples, and reviews. The developer was solely responsible for applying all file changes.
- **Current scope — 2026-10-22 clarification:** Production code, assets and settings remain developer-applied. Explicitly requested documentation updates are allowed under AI-003. This amendment does not authorize autonomous production edits.
- **Rationale:** Keeps implementation and approval under the developer's control while reducing repeated manual copy/paste of code into chat.
- **Implications:** Reviews remain read-only; C# examples are delivered in chat. Documentation updates are suggested first and applied only when explicitly requested, subject to environment permissions (AI-003).
- **Scope:** AI collaboration process; **not** an assertion about gameplay architecture.

## AI-002 — Store durable design context in Markdown

- **Date:** 2026-10-22
- **Status:** Accepted (2026-10-22 developer confirmation).
- **History:** Initially Proposed, awaiting adoption. The developer subsequently requested applying the updated context documents and proposing future updates when useful.
- **Context:** A Codex conversation alone is not a guaranteed permanent or complete record of previous design rationale.
- **Decision:** Use `PROJECT.md`, `ARCHITECTURE.md`, `CURRENT.md`, `DECISIONS.md`, and `SESSIONS.md` in `Docs/AI/`.
- **Rationale:** Makes accepted decisions and active context discoverable across Codex sessions and reviewable in Git.
- **Maintenance:** Suggest updates when goals, decisions or verified implementation change. Apply only within an explicitly authorized documentation scope (AI-003).

---

## AI-003 — 명시적으로 요청한 문서 갱신 허용

- **Date:** 2026-10-22.
- **Status:** Accepted.
- **Context:** 개발자는 분석·확인된 요구사항을 문서에 실제 반영하고,
  앞으로 AI Docs 수정이 유용한 경우 먼저 제안하도록 요청했다.
- **Decision:** 이번 AGENTS.md 및 Docs/AI 기준 문서 갱신을 허용한다.
  향후에는 갱신 필요성을 제안하고 명시적 반영 요청 범위에서만 편집한다.
- **Rationale:** 개발자의 구현 통제권을 유지하면서 다음 세션의 문맥을 최신화한다.
- **Consequences:** AI-001의 문서까지 항상 수동 반영하던 정책만 한정적으로 보완한다.
  생산 코드·자산·설정 변경, Unity 실행, 빌드·테스트는 허용하지 않는다.
  파일시스템 권한 절차도 별도로 준수한다.
- **Affected files:** AGENTS.md, Docs/AI/.
- **Verification:** 이번 대화의 개발자 명시 요청. 미래 자동 편집의 포괄 승인은 아님.

## Gameplay architecture decisions

The initial context contained no confirmed gameplay ADRs. The decisions below were explicitly confirmed by the developer on 2026-10-22. They record intent, not completed implementation. Do not import additional decisions from the reference game, external repositories or personal notes.

## ADR-001 — 테스트 단계의 맵 연결 책임

- **Date:** 2026-10-22.
- **Status:** Accepted.
- **Context:** 맵 생성 기본 틀 이후 MapView를 작성하고 Editor 확인을 준비 중이다.
- **Decision:** 현재 생성·표시·입력 연결은 MapTestScene에서 담당한다.
  테스트가 어느 정도 진행된 후 다른 곳으로 이전한다.
- **Rationale:** 개발자가 현재 테스트 단계의 연결 위치로 지정했다.
- **Implementation:** MapTestScene.Start/Clear는 비어 있고 씬 연결은 아직 없다.
  이는 개발자가 설명한 코드 작업 단계와 일치한다.
- **Open details:** 이전 위치와 구체 시점은 미정이다.
- **Affected files:** Assets/_Projects/Scripts/Map/Test/MapTestScene.cs,
  MapManager.cs 및 MapView.cs(동일 Map 디렉터리).

## ADR-002 — 맵 생성의 필수 규칙

- **Date:** 2026-10-22.
- **Status:** Accepted.
- **Decision:** 마지막 노드는 반드시 Boss Enemy다.
  모든 시작점에서 종착점으로 가는 경로가 있어야 하며 막다른 길은 허용하지 않는다.
- **Rationale:** 개발자가 확정한 맵 진행 요구사항이다.
- **Implementation:** 현재 생성기는 마지막 레이어를 종착점으로 선택하지만
  BossEnemy를 강제하지 않고 교차 제거 뒤 도달 가능성을 검증하지 않는다.
- **Consequences:** 생성 기본 틀이 있다는 사실을 규칙 검증 완료로 해석하지 않는다.
  테스트에서 타입, 시작점별 종착점 도달, 중간 막다른 길을 확인한다.
- **Configuration:** MapConfig.asset은 임시 값이며 개발자가 테스트에서 수정한다.
  현재 값과 규칙의 차이를 규칙 철회로 해석하지 않는다.
- **Affected files:** Assets/_Projects/Scripts/Map/MapGenerator.cs,
  Assets/_Projects/Scripts/Map/MapConfigSO.cs,
  Assets/Resources/ScriptableObject/Map/MapConfig.asset.

## ADR-003 — 런 상태와 씬 수명 분리

- **Date:** 2026-10-22.
- **Status:** Accepted.
- **Decision:** 시드와 맵 진행은 씬 전환 후에도 유지한다.
  향후 RunManager를 두어 로그라이크 게임 한 번의 런을 관리한다.
- **Rationale:** 씬 전환이 같은 런의 시드·맵 진행을 초기화해서는 안 된다.
- **Implementation:** RunManager는 아직 구현되지 않았다.
  현재 RuntimeManager는 등록되지 않았고 MapManager는 고정 시드를 사용한다.
- **Conflict to address:** SceneManagerEX.LoadScene은 Managers.Clear를 호출하며
  RuntimeManager.Clear는 Seed와 Random을 지운다.
  향후 시스템을 단순 등록하기 전에 씬 정리와 런 종료의 경계를 정해야 한다.
- **Open details:** RunManager API·소유권·등록 방식·런 종료/재시작,
  RuntimeManager의 대체·통합 여부는 미정이다.
- **Non-decision:** 게임 종료 뒤 디스크 저장·복원까지 승인한 것은 아니다.
- **Affected files:** Assets/_Projects/Scripts/System/RuntimeManager.cs,
  Assets/_Projects/Scripts/Map/MapManager.cs,
  Packages/urdev-game-framework/Runtime/Scripts/Core/Bootstrap/Managers.cs,
  Packages/urdev-game-framework/Runtime/Scripts/Systems/Scene/SceneManagerEX.cs.

## ADR-004 — 맵 풀링과 진행 중 지도 재열기

- **Date:** 2026-10-22.
- **Status:** Accepted.
- **Decision:** 맵을 풀링하여 게임 진행 중 현재 지도를 다시 볼 수 있게 한다.
- **Rationale:** 진행 중 현재 맵을 재사용하여 확인하는 기능이 필요하다.
- **Implementation:** MapView는 현재 Clear 후 Instantiate하며 Clear에서 Destroy한다.
  공용 PoolManager는 있지만 MapView와 연결되어 있지 않다.
- **Open details:** 전체 맵·노드·선 중 풀링 단위, 반환 시점,
  씬 간 표시 객체 수명 및 공용 풀 사용 방식은 미정이다.
- **Consequences:** 표시 재사용과 런 데이터 보존을 분리해 검토한다.
  이번 결정은 현재 화면 테스트 전에 풀링부터 구현해야 한다는 일정 승인이 아니다.
- **Affected files:** Assets/_Projects/Scripts/Map/MapView.cs,
  Assets/_Projects/Scripts/Map/MapNode.cs,
  Assets/_Projects/Scripts/Map/MapLine.cs,
  Packages/urdev-game-framework/Runtime/Scripts/Systems/Pooling/PoolManager.cs.

## ADR-005 — 자체 프레임워크 명칭 통일

- **Date:** 2026-10-22.
- **Status:** Accepted.
- **Decision:** 자체 제작 프레임워크의 통일 명칭은 `URDev.GameFrameWork`다.
  대소문자는 개발자가 제시한 표기를 따른다.
- **Rationale:** 개발자가 자신의 프레임워크 명칭을 지정했다.
- **Implementation:** 현재 asmdef/주 네임스페이스는 `URDev.GameFramework`,
  Framework.cs의 네임스페이스는 `UR.GameFramework`다.
- **Consequences:** 목표 명칭과 실제 소스 심볼을 분리해 인용한다.
  이번 갱신은 문서만 변경하며 코드·asmdef·직렬화 자산을 이름 변경하지 않는다.
- **Open details:** 코드 이름 이전의 범위·시점과 호환성 검토는 개발자가 정한다.
  package ID 변경·외부 배포·지원 버전 확대는 이번에 결정하지 않았다.
- **Affected files:** Packages/urdev-game-framework/Runtime/URDev.GameFramework.asmdef,
  Packages/urdev-game-framework/Runtime/Framework.cs,
  Packages/urdev-game-framework/Runtime/Scripts/,
  Packages/urdev-game-framework/README.md,
  Packages/urdev-game-framework/Docs/PackageAuthoringGuide.md.

## OBS-001 — 현재 구현과 참고 자료의 지위

- **Date:** 2026-10-22.
- **Status:** Observed.
- **Implementation:** 모델·생성·진행·표시 분리, IRandom 주입,
  Managers 기반 서비스 접근과 IGameSystem/IUpdatableSystem 생명주기를 확인했다.
- **Meaning:** 패턴의 존재가 별도의 공식 채택 이유나 전면 리팩터링 승인은 아니다.
- **Reference policy:** Assets/_Projects/Temp/Memo.md는 개인 고민의 참고 자료다.
  저장·hover·경로 표시·난수 분리 등 메모 항목을 현재 확정 요구로 격상하지 않는다.
  풀링은 메모가 아니라 이번 개발자 명시 확인을 근거로 ADR-004에 기록했다.
- **External reference:** 개발자가 지정한
  https://github.com/alis7430/slay-the-spire-map-in-unity 는 구현 참고 자료다.
  프로젝트의 현재 소스나 개발자의 명시적 요구보다 우선하지 않는다.

### New decision template

```markdown
## ADR-XXX — [Decision title]

- **Date:** YYYY-MM-DD
- **Status:** Proposed | Accepted | Superseded | Rejected
- **Context:** [What issue motivates the decision?]
- **Decision:** [What was chosen?]
- **Alternatives:** [Other designs and their trade-offs]
- **Rationale:** [Why this choice?]
- **Consequences:** [Advantages, limitations, migration impacts]
- **Affected files/systems:** [Verified paths and names]
- **Verification:** [Evidence and tests, if any]
```
