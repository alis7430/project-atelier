# Project Atelier

Slay the Spire 2를 참고하여 개발 중인 **Unity 기반 덱빌딩 로그라이크 프로토타입**입니다.

현재는 맵 생성, 경로 선택, 진행 상태 표시를 중심으로 개발하고 있습니다.

## 현재 구현

- 시드 기반 레이어형 노드 맵 생성
- ScriptableObject 기반 맵 설정
- 노드와 연결선 UI 표시
- 이동 가능한 노드 판정 및 방문 이력 관리
- 현재 맵의 진행을 유지하는 화면 재표시

## 개발 환경

- Unity 6000.3.6f1
- C#
- Universal Render Pipeline (URP)
- Input System / uGUI
- 자체 제작 프레임워크 `URDev.GameFramework`

## 실행 방법

1. 저장소를 클론합니다.
2. Unity Hub에서 Unity 6000.3.6f1로 프로젝트를 엽니다.
3. `Assets/_Projects/Scenes/MapTestScene.unity`를 엽니다.
4. Play Mode에서 맵 생성과 노드 선택을 확인합니다.

## 개발 예정

- 씬 전환 시 시드와 맵 진행 상태 유지
- 한 번의 플레이를 관리하는 RunManager
- 맵 표시 객체 풀링

> 개발 중인 프로토타입으로, 기능과 구조는 변경될 수 있습니다.
