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
- 자체 프레임워크 통일 명칭은 URDev.GameFrameWork다.
- 이번 문서 반영을 요청했고 향후 필요한 AI Docs 수정은 먼저 제안하도록 했다.

### Decisions

- AI-001: 생산 코드에 대한 읽기 전용 조언 역할 유지.
- AI-002: Markdown 컨텍스트 유지 방식 채택.
- AI-003: 명시적으로 요청한 범위의 문서 갱신 허용.
- ADR-001: MapTestScene의 임시 연결 책임.
- ADR-002: 맵 필수 규칙.
- ADR-003: 런 상태의 씬 간 유지와 향후 RunManager.
- ADR-004: 맵 풀링과 재열기.
- ADR-005: URDev.GameFrameWork 명칭 통일.
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
- 실제 프레임워크 코드의 명칭 이전.
- 향후 문서 갱신 필요 시 제안하고 명시적 반영 요청에 따라 수행.

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
