# Project Atelier — AI Collaboration Session Log

> Maintain **brief, decision-relevant summaries**, not raw conversation transcripts. Add entries only after the developer reviews and accepts them.

## 2026-10-22 — Read-only Codex advisory workflow

### Context

The developer described using ChatGPT on the web to explain Project Atelier's code situation and receive architecture/design and C# suggestions. Repeated manual description of code and losing continuity across discussions were pain points.

### Established requirements

- AI should refer to the current Unity project's codebase directly.
- AI should review existing code and provide example implementations in chat.
- **No autonomous file edits.** The developer writes and integrates code.
- AI should retain important design intent and active-work context between sessions.

### Proposed solution

- Use Codex CLI from the Unity project root in `--sandbox read-only` mode.
- Define stable AI behavior in `AGENTS.md`.
- Store project overview, architecture map, active work, decisions, and sessions in `Docs/AI/`.
- Optionally add reusable task-specific skills for architecture review, code review, and feature design.
- Have Codex draft memory/document changes in chat only; developer applies approved updates.

### Outcomes / status

- Read-only advisory role: **agreed requirement**.
- The Markdown structure: **recommended; initial files drafted**.
- Actual Project Atelier implementation, Unity version, and currently active gameplay feature: **not yet verified**.

### Next steps

1. Put the files in the Project Atelier repository.
2. Let Codex inspect actual code read-only and propose baseline architecture updates.
3. Record the first verified system map and first gameplay ADR.

---

## 2026-10-22 — 프로젝트 정적 분석과 개발자 요구사항 확정

### Goal

실제 소스와 AI 문서를 대조하고, 개발자 답변으로 제품 목표와
현재 맵 작업 범위를 확정하여 문서에 반영한다.

### Source reviewed

- AGENTS.md 및 Docs/AI의 5개 문서.
- Assets/_Projects/Scripts의 C# 18개.
- Packages/urdev-game-framework/Runtime의 C# 21개 및 asmdef.
- Packages/manifest.json, packages-lock.json, 프레임워크 package.json.
- Unity 버전, 입력·렌더링·빌드 씬 설정.
- MapTestScene.unity, EventSystem.prefab, 맵 ScriptableObject 및 관련 GUID.
- 프레임워크 README/작성 가이드/CHANGELOG.
- 개인 참고 메모 Assets/_Projects/Temp/Memo.md.
- 후속 갱신 시 MapManager, MapView, MapTestScene, RuntimeManager,
  프레임워크 asmdef와 AI 문서를 다시 확인했다.

생성 루트 Library/Temp/Obj/Logs는 분석에서 제외했다.
외부 참고 URL 본문은 가져오지 못했으며 해당 구현을 검증했다고 주장하지 않는다.

### Initial findings

- Unity 6000.3.6f1, URP 17.3.0, Input System 1.18.0, uGUI 2.0.0.
- 별도 게임 asmdef 없이 게임 코드가 있고 공용 프레임워크는 분리되어 있다.
- 맵 모델·생성·방문·표시 코드가 존재하나 씬 연결은 없다.
- RuntimeManager 등록 및 런타임 RNG 사용 연결은 없다.
- MapConfig 자산과 코드 필드명이 다르다.
- 프레임워크 이름과 문서 경로에 불일치가 있다.
- 기존 AI 문서는 초기 미검증 템플릿이었다.

### Developer clarification

- 게임은 Slay the Spire 2를 레퍼런스로 하는 덱빌딩 로그라이크다.
- 전체 플로우는 레퍼런스와 거의 같은 방향이며 프로토타입을 목표로 한다.
- 개발자가 일러스트레이터 1명과 협업한다.
- 현재 맵 생성 기본 틀 이후 MapView를 작성하고 Editor 확인을 준비 중이다.
- 맵 구현 참고는 https://github.com/alis7430/slay-the-spire-map-in-unity 이다.
- 지금은 코드 작업 단계이며 씬 미연결은 알려진 상태다.
- 초기 연결은 MapTestScene에 두고 테스트 후 옮긴다.
- 종착점 Boss Enemy, 모든 시작점의 종착점 도달, 막다른 길 금지가 필수다.
- MapConfig의 현재 값은 임시이며 테스트에서 수정한다.
- 시드·맵 진행은 씬 간 유지하며 향후 RunManager가 한 번의 런을 관리한다.
- 맵을 풀링하여 진행 중 현재 지도를 다시 열 수 있게 한다.
- 개인 Memo.md는 참고용이며 확정 계획으로 취급하지 않는다.
- 당시 프레임워크 명칭은 URDev.GameFrameWork로 기록했다. 같은 날 후속 정정으로 대체되었으며 아래 기록과 ADR-005를 따른다.
- 이번 문서 반영을 요청했고 향후 필요한 AI Docs 수정은 먼저 제안하도록 했다.

### Decisions

- AI-001: 생산 코드에 대한 읽기 전용 조언 역할 유지.
- AI-002: Markdown 컨텍스트 유지 방식 채택.
- AI-003: 명시적으로 요청한 범위의 문서 갱신 허용.
- ADR-001: MapTestScene의 임시 연결 책임.
- ADR-002: 맵 필수 규칙.
- ADR-003: 런 상태의 씬 간 유지와 향후 RunManager.
- ADR-004: 맵 풀링과 재열기.
- ADR-005: 프레임워크 명칭 통일 (최신 표기는 아래 후속 정정 참조).
- 세부 API, 풀 단위, 최종 연결 위치를 추가로 승인받았다고 가정하지 않는다.

### Documentation changes

- AGENTS: 생산 코드 읽기 전용과 명시적 문서 갱신 예외 구분,
  실제 소스 탐색 경로·컨벤션·현재 목표 반영.
- PROJECT: 제품 목표, 협업 구성, 레퍼런스와 검증된 기술 스택.
- ARCHITECTURE: 현재 구조·수명·흐름과 승인된 미구현 방향 분리.
- CURRENT: MapView와 Editor 확인을 우선 작업으로 기록.
- DECISIONS: 기존 정책의 이유·이력 보존, 명시적 승인만 Accepted로 기록.
- SESSIONS: 최초 협업 기록 보존, 이번 분석과 후속 확인 추가.

### Validation limits

Unity 실행, 빌드, 테스트, 프로파일링을 수행하지 않았다.
생산 코드·씬·자산·설정은 이번 문서 갱신의 변경 범위가 아니다.
기존 MapView.cs 미커밋 변경을 개발 목표의 근거로 사용하지 않았다.
현재 목표는 개발자의 직접 설명으로 확정했다.

### Follow-ups

- MapView 작성과 MapTestScene 연결 후 개발자의 Editor 확인.
- 임시 MapConfig 정리 및 맵 규칙 검증.
- 연결 책임 이전 시점과 위치.
- RunManager 소유권·수명·RuntimeManager 관계.
- 풀링 단위와 표시 객체 수명.
- 당시 후속 항목인 프레임워크 명칭 이전은 아래 정정 및 소스 확인으로 해소되었다.
- 향후 문서 갱신 필요 시 제안하고 명시적 반영 요청에 따라 수행.

---

## 2026-10-22 — 프레임워크 명칭 정정 및 소스 확인

- **개발자 결정:** 통일 명칭은 `URDev.GameFramework`다. 이전 `URDev.GameFrameWork` 표기를 대체한다.
- **소스 확인:** 개발자가 변경한 `Packages/urdev-game-framework/Runtime/Framework.cs`의 namespace와 `Runtime/URDev.GameFramework.asmdef`의 name/rootNamespace가 모두 `URDev.GameFramework`다.
- **문서 반영:** AGENTS.md, Docs/AI, 관련 로컬 스킬 지침과 패키지 README/작성 가이드의 명칭을 정정했다. ADR-005를 갱신했으며 명칭 이전을 미완료로 취급하지 않는다.
- **범위:** 문서만 수정했다. 패키지 ID 변경, 생산 코드 변경, Unity 실행·빌드·테스트는 수행하지 않았다.

---

## 2026-10-24 — macOS 컨텍스트 복원과 맵 구현 진전 기록

### Goal

Git으로 보존된 문서와 실제 소스·자산을 대조하여 Windows에서 이어진 개발 맥락을 복원한다.
복원 보고 후 개발자가 확인된 진행 상황의 문서 반영을 명시적으로 요청했다.
이전 대화 원문을 기억하거나 접근했다고 가정하지 않는다.

### Source reviewed

- AGENTS.md, Docs/AI의 5개 문서, 로컬 architecture-review/code-review/feature-design 지침.
- Assets/_Projects/Scripts의 맵·난수·RuntimeManager·유틸리티 코드.
- 프레임워크 README/CHANGELOG/작성 가이드, asmdef 및 핵심 서비스·생명주기 코드.
- MapTestScene.unity, MapNode/MapLine 및 EventSystem 프리팹,
  MapConfig와 MapNodeSO 자산 및 관련 .meta GUID/fileID.
- Unity 버전, 패키지 선언, 빌드 씬·입력 설정, 관련 Git 이력. 소스 기준 HEAD: 81014b0.

### Findings

- AI 기준 문서의 마지막 커밋 d5a876b 이후 880f377에서 맵 프리팹과 Node 하위 자산이 추가되고,
  20b475d에서 MapView 확장·MapTestScene 연결·MapConfig 필드 정합성이 반영되었다.
- MapTestScene.Start는 씬의 _seed(저장값 99999)로 생성한 맵을 SetMap → ShowMap →
  최초 RefreshStates 순으로 연결한다. 클릭 이벤트의 구독·해제, ID 전달과 방문 처리도 존재한다.
- 저장된 씬에 Config/Manager/View 및 노드·선 프리팹 참조가 연결되어 있다.
  초기 문서의 “씬 미연결”은 당시 상태이며 현재 상태로 사용하지 않는다.
- MapView는 노드와 간선 상태 색상을 갱신하고 방문 노드/간선 HashSet을 재사용한다.
  Clear는 구독 해제, 표시 객체 비활성화·파괴 및 컬렉션 정리를 수행한다.
- MapConfig의 필드명은 현재 코드와 일치하고 마지막 레이어는 BossEnemy,
  randomNodeValue=0이다. 이는 생성기의 모든 설정에 대한 Boss 강제나 경로 보장이 아니다.
- 81014b0에서 연결된 노드만 반환하는 필터가 복원되었고 노드 아이콘 크기는 75에서 50으로
  조정되었다. 필터 변경의 별도 설계 이유는 기록으로 확인하지 못했다.
- RunManager, RuntimeManager 등록, 씬 간 진행 보존과 맵 풀링 연결은 발견하지 못했다.
  교차 연결 제거 후 도달 가능성 검증·보정도 없다.

### Decisions

- 기존 AI 정책 및 ADR-001~005의 승인된 의도는 유지한다. 새 게임 아키텍처 결정은 없다.
- 이번 요청은 파악한 진행 상황의 문서 반영을 허용한다. 생산 코드·자산·설정 변경이나
  향후 문서 자동 수정의 포괄 승인은 아니다.

### Documentation changes

- CURRENT/PROJECT: 테스트 연결이 구현된 현황과 실행 검증의 한계를 반영했다.
- ARCHITECTURE: 현재 호출·이벤트 흐름, 씬/프리팹 연결, 설정 정합성과 표시 상태를 갱신했다.
- DECISIONS: 기존 ADR의 구현 상태를 보완하고 결정 당시 맥락과 이유는 보존했다.
- AGENTS: 오래된 진행 상태 요약을 최신 정적 확인 결과와 맞췄다.
- SESSIONS: 과거 기록을 유지하고 이번 복원 근거와 미해결 사항을 추가했다.

### Validation limits / environment

- Unity 실행·빌드·테스트·프로파일링은 수행하지 않았다. 개발자의 Editor 검증 완료 여부도
  저장소 근거만으로는 확정하지 않는다. 코드 존재, 저장 참조, 실제 동작을 구분한다.
- macOS에서 git-lfs 명령을 찾지 못해 기본 Git 상태 조회가 실패했다.
  명령 단위로 LFS 필터를 끈 조회에서 변경으로 표시된 LiberationSans.ttf와 EmojiOne.png는
  실제 내용의 SHA-256이 HEAD LFS OID와 일치했다. 사용자 수정으로 분류하지 않았다.
  Git 설정을 변경하거나 패키지를 설치하지 않았으며 전체 LFS 자산 복원 여부는 미검증이다.
- 외부 참고 저장소 구현은 이번 복원에서 검증하지 않았다.

### Follow-ups

- 개발자의 Editor 화면·입력·노드/간선 상태·재표시 검증 결과 확인.
- 여러 설정과 시드에서 Boss 종착점, 시작점별 도달 가능성, 막다른 길 금지 검증.
- RunManager 소유권·수명·RuntimeManager 관계, 풀링 단위와 최종 연결 위치 결정.
- 일부 로컬 스킬의 “씬 미연결” 설명도 과거 상태다. 사용 시 최신 소스와 CURRENT를 우선한다.
  이번에는 스킬 파일과 패키지 문서를 수정하지 않았다.

---

## 2026-10-26 — 맵 재표시·정리 안정화 구현 및 개발자 테스트 보고

### Goal

같은 맵 재표시, 새 맵 생성, 화면 비활성화/재활성화, 표시 정리 후 재표시를 구분하여 설계했다.
개발자는 제안을 수용해 코드를 수정하고 테스트 결과 문제없음을 보고했으며 문서 반영을 요청했다.

### Source reviewed

- MapTestScene.cs, MapManager.cs, MapView.cs, MapGenerator.cs.
- RuntimeManager.cs, SceneManagerEX.cs, MapTestScene.unity의 저장된 _seed.
- Docs/AI의 PROJECT/CURRENT/ARCHITECTURE/DECISIONS 및 관련 이전 세션 기록.
- 현재 작업 트리의 소스를 기준으로 확인했다(조회 HEAD: 2c9c332).

### Findings

- Start는 GenerateNewMap을 호출한다. 생성 성공 시에만 SetMap으로 맵·진행을 초기화한다.
- RedisplayCurrentMap은 현재 Map으로 ShowMap → RefreshStates를 호출하며 진행을 초기화하지 않는다.
- ClearDisplayedMap은 표시만 정리하고 클릭 구독과 진행을 유지한다.
  기존 Clear는 구독 해제까지 수행하는 씬 정리 경로다.
- 세 ContextMenu 진입점은 Play Mode에서만 동작한다.
- 생성기에는 마지막 레이어 Boss 강제, 입력 설정 검증 및 교차 제거 후 ValidatePaths가 있다.
  이전 문서의 “Boss 강제·경로 검증 없음”은 과거 상태로 정정했다.
- 현재 저장된 테스트 씬의 시드는 12312다. 2026-10-24의 99999 기록은 당시 이력으로 보존한다.

### Decisions

- 기존 ADR-001의 테스트 연결 책임 안에서 생성·재표시·표시 정리 진입점 분리를 개발자가 수용했다.
- RunManager의 구체 소유권·등록 방식, 풀링 단위·반환 정책은 이번에 결정하지 않았다.

### Validation

- 개발자 보고: 위 안정화 코드를 수정한 뒤 테스트 결과 문제없음.
- 세부 항목별 로그·반복 횟수·시드/설정 목록은 제공되지 않았다.
- AI는 정적 소스 확인과 문서 편집·차이 검토만 수행했으며 Unity·빌드·테스트를 실행하지 않았다.
- 생성 규칙의 여러 시드·설정 검증, 씬 전환 후 보존, 풀링 검증은 완료로 추정하지 않는다.

### Documentation changes

- CURRENT/PROJECT에 구현 상태와 개발자 테스트 보고를 반영했다.
- ARCHITECTURE에 호출 경로·정리 경계와 현재 생성기 검증을 반영했다.
- DECISIONS의 기존 ADR-001~004에 수용된 범위와 구현 상태를 보완했다.
- 생산 코드·씬·자산·프로젝트 설정 및 AGENTS.md는 수정하지 않았다.

### Follow-ups

1. 여러 시드·설정에서 생성 규칙 및 실패 시 기존 맵·진행 유지 확인.
   기존 MapManager.ValidateMapSeeds 실행 여부·결과와 실패 시드 기록도 확인한다.
2. RunManager의 시드·맵·진행 소유권, RuntimeManager와의 관계,
   씬 정리와 런 종료 경계를 설계하고 씬 전환 후 복원 확인.
3. 풀링 단위와 대여·반환 시 상태/이벤트 정책 설계 후 통합.
4. 확인 결과에 따라 표현·설정 및 MapTestScene 이후 연결 책임 위치 결정.

---

## New session entry template

```markdown
## YYYY-MM-DD — [Topic]

### Goal
[What problem was discussed?]

### Source reviewed
[Verified files / classes]

### Findings
[Concise code-grounded observations]

### Decisions
[Only explicit developer approvals; link to DECISIONS.md]

### Follow-ups
[Outstanding work and validation items]
```
