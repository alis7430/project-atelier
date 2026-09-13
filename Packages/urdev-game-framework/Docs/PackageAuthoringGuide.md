# Unity Script-Only Package Authoring Guide

이 문서는 `URDev Game Framework` 패키지를 C# 스크립트만으로 확장할 때 지켜야 할 기본 규칙을 정리합니다.

## 1. 패키지 위치

패키지는 프로젝트의 `Packages/com.urdev.game-framework` 폴더에 둡니다.

Unity Package Manager는 이 폴더의 `package.json`을 읽어 패키지 이름, 버전, Unity 최소 버전, 의존성을 인식합니다.

## 2. 기본 폴더 규칙

- `Runtime`: 게임 실행 중 사용하는 C# 코드
- `Editor`: 에디터 전용 C# 코드가 필요할 때만 추가
- `Tests`: 테스트 코드가 필요할 때만 추가
- `Samples~`: 예제 코드가 필요할 때만 추가
- `Docs`: 패키지 제작 및 사용 문서

현재 패키지는 스크립트 전용 프레임워크이므로 텍스처, 프리팹, 씬, 모델, 오디오 같은 에셋을 넣지 않습니다.

## 3. 네임스페이스 규칙

기본 네임스페이스는 다음을 사용합니다.

```csharp
namespace UR.GameFramework
{
}
```

기능별 코드가 커지면 다음처럼 하위 네임스페이스를 사용합니다.

```csharp
namespace UR.GameFramework.Core
{
}

namespace UR.GameFramework.Services
{
}

namespace UR.GameFramework.StateMachines
{
}
```

Unity의 전역 네임스페이스에는 타입을 만들지 않습니다.

## 4. Assembly Definition 규칙

런타임 코드는 `Runtime/URDev.GameFramework.asmdef`에 포함됩니다.

- Assembly name: `URDev.GameFramework`
- Root namespace: `URDev.GameFramework`
- Auto Referenced: enabled
- No Engine References: disabled

에디터 전용 코드가 필요해지면 `Editor/URDev.GameFramework.Editor.asmdef`를 별도로 만들고 `includePlatforms`를 `Editor`로 제한합니다.

## 5. 새 코드 추가 절차

1. `Runtime` 아래에 기능별 폴더를 만듭니다.
2. 새 C# 파일을 추가합니다.
3. 모든 public 타입을 `URDev.GameFramework` 또는 하위 네임스페이스 안에 둡니다.
4. 외부 패키지가 필요하면 `package.json`의 `dependencies`에 명시합니다.
5. 패키지 버전을 바꿀 정도의 변경이면 `CHANGELOG.md`를 업데이트합니다.

## 6. 버전 관리 규칙

`package.json`의 `version`은 SemVer 형식을 사용합니다.

- `0.1.0`: 초기 개발 버전
- `0.x.y`: API가 안정화되기 전의 개발 버전
- `1.0.0`: 외부 프로젝트에서 안정적으로 사용할 수 있는 첫 공개 버전

코드에서 노출하는 `Framework.Version` 값도 패키지 버전과 함께 갱신합니다.

## 7. 다른 프로젝트에서 사용하기

Git 저장소로 배포할 경우 Unity Package Manager에서 다음 형식으로 추가할 수 있습니다.

```text
https://github.com/{owner}/{repository}.git?path=Packages/com.urdev.game-framework
```

로컬 프로젝트에서 테스트할 때는 Package Manager의 `Add package from disk...`를 선택한 뒤 이 패키지의 `package.json`을 지정합니다.
