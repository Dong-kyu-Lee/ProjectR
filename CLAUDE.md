# ProjectR

**무너진 왕국, 살아남은 사람들의 반격** — 도트 그래픽 2D 로그라이크 액션 RPG.
왕국 지하의 술집에서 출발해, 서로 다른 직업의 생존자들이 무작위 던전을 탐험하며 재앙의 원인을 찾아 올라간다.

## 게임 개요

핵심 루프: 술집(캐릭터 선택·고유 능력 해방) → 던전 탐험/웨이브 전투 → 경험치·아이템·재화로 빌드 → 상점·보스 → 다음 스테이지 또는 사망 시 술집 복귀

- 사망 시 **런 성장(아이템·스탯·레벨)은 초기화**, **해방한 고유 능력은 유지**된다. 데이터를 다룰 때 이 경계를 지킬 것.
- 플레이어블 직업: Bartender(투척), Blacksmith(근접). 의사 등 추가 직업 예정.
- 기획의 핵심 축: 직업별 전투 / 던전 탐험(방·적 구성 무작위) / 한 판의 성장 / 왕국의 이야기

## 환경

- Unity **6000.3.7f1**, URP 2D (`com.unity.render-pipelines.universal` 17.3)
- **레거시 Input Manager** 사용 (`Input.GetKey` 등). 새 Input System 코드를 작성하지 않는다.
- Cinemachine **2.x** (3.x API 아님), Timeline(컷신), TextMeshPro(한글 폰트)
- 패키지 추가·업그레이드는 요청받았을 때만 한다.

## 씬 흐름

빌드 씬(EditorBuildSettings): StartScene → Prologue → LobbyScene(술집) → DungeonGenerate → TempMiddleBoss → ShopScene → TempFinalBoss → EndScene (+ TestCutScene, TestFinalBossCutScene)

- 씬 종류는 `Define.SceneType`, 씬 전환은 `GameManager`를 통해 한다.
- 그 외 `*Test*`, `*Prototype*` 등의 씬은 개인 작업·실험용이다. 팀 규칙상 **작업할 씬을 따로 만들어 그 씬에서 작업**한다.

## 아키텍처

- **싱글톤 매니저** (`Instance` + `DontDestroyOnLoad`): GameManager, PlayerManager, SoundManager, SaveManager, DungeonFlowManager, InGameUIManager, AbilityManager, BuffEffectManager, StorySystem 등.
  - 싱글톤 매니저를 새로 설계하거나 수정할 때 기존의 `Instance` 접근 방식은 유지한다. **`DontDestroyOnLoad` 적용 여부는 먼저 사용자에게 물어본다.**
- **플레이어는 `Assets/Scripts/Player/V2/`가 현재 기준** (`PlayerControllerBase` → `BartenderControllerV2`, `BlacksmithControllerV2`). 프리팹은 `Resources/Prefabs/Player Prefabs/`.
  - `Assets/Scripts/` 루트의 `PlayerController.cs`, `BartenderController.cs` 등은 **레거시**로, `Prefabs/Legacy/`와 테스트 씬에서만 쓰인다. 수정하지 말 것.
- **`Define.cs`**: 여러 클래스에서 공유하는 "종류" enum(적, 씬, 캐릭터 등)만 둔다. 한 클래스에서만 쓰는 정의는 넣지 않는다.
- **상태 머신**: 적 AI는 `Enemy/StateMachine`(`IState`), 공격 방식은 `Enemy/AttackStrategy`. 스테이지 흐름은 `DungeonScripts/StageFlow`(StageGraph/StageNode/StageStates). 설계 문서: `Docs/StageFlow.html`
- 데이터(적, 스테이지, 스토리, 캐릭터 등)는 ScriptableObject로 관리한다.
- 버프·아이템은 `BuffFactory` / `ConcreteBuffs`, `ItemScripts/EquipmentItems/ConcreteEquips` 등의 구현체를 추가해 확장한다.
- 반복 생성되는 오브젝트는 `ObjectPoolManager`를 사용한다.

## 도트 그래픽

- 스프라이트 Filter Mode는 **Point(no filter)**, 압축은 None.
- PPU는 에셋마다 100/32/16 등으로 섞여 있다. 새 스프라이트는 같은 묶음의 기존 에셋과 맞춘다. (TODO: 캐릭터·타일 기준 PPU 확정)
- Pixel Perfect Camera는 현재 쓰지 않는다.

## Unity 작업 주의사항

- `.meta` 파일은 원본과 항상 함께 이동·이름 변경·삭제·커밋한다.
- `.unity` / `.prefab` / `.asset` YAML은 직접 수정하지 않는다. 필요하면 먼저 설명하고 확인받는다.
- **여러 명이 쓰는 프리팹은 직접 수정하지 않고 프리팹 배리언트를 만들어 수정한다.**
- `[SerializeField]` 필드 이름을 바꿀 때는 `[FormerlySerializedAs("oldName")]`를 붙여 인스펙터 값이 날아가지 않게 한다.
- `Resources.Load`가 경로 문자열을 쓰므로 `Resources/` 아래 에셋을 옮기거나 이름을 바꾸지 않는다. **Resources에는 실제로 사용할 에셋만 넣는다**(빌드 크기).
- `Library/`, `Temp/`, `obj/`, `Logs/`, `*.csproj`, `*.sln`은 자동 생성물이므로 수정하지 않는다.
- **Unity Editor를 다룰 때(씬·에셋 편집, C# 실행, 빌드, 테스트, 컴파일 확인 등)는 `unity-cli` 스킬을 사용한다.** 스킬로 확인하지 못한 변경은 사용자에게 확인을 요청한다.

## 코딩 컨벤션

- 들여쓰기 스페이스 4칸, CRLF (`.editorconfig`).
- **함수 시그니처 위에 함수 설명을 한글 주석으로 적는다.** (Unity 이벤트 함수, getter/setter 같은 간단한 함수는 제외)
- 이름은 짧게 줄이지 말고 기능이 드러나게 짓는다.

| 대상 | 규칙 | 예시 |
| --- | --- | --- |
| private 멤버 변수 | camelCase | `privateValue` |
| public 멤버 변수 | camelCase | `publicValue` |
| 프로퍼티 | PascalCase | `VelocityValue` |
| enum | PascalCase 타입·멤버 | `EnumType { First, Second }` |
| 함수 | PascalCase, 매개변수 camelCase | `FuncHello(int firstValue)` |
| 클래스 | PascalCase | `ClassHello` |
| 이벤트 | `On` 접두사 | `OnSceneChanged` |

- 현재 namespace와 asmdef는 사용하지 않는다.
- `Update()` 안에서 `GetComponent` / `Find` 호출을 피하고 캐싱한다.

## Git

Git flow를 따른다.

- `master`: 출시 버전. develop → master merge만 한다.
- `develop`: 개발 중심 브랜치. **직접 커밋하지 않는다.**
- `feature/(기능명)`: develop에서 분기한다 (예: `feature/PlayerMove`).

**커밋**: [commit 스킬](.claude/skills/commit/SKILL.md)의 규칙을 따른다. 한글 제목과 이슈 번호, 본문에 변경사항 번호 목록.

**PR** (요청받았을 때만 생성):
- **base는 `develop`**, compare는 작업 브랜치. (기본 브랜치가 master이므로 반드시 base를 지정한다.)
- 제목은 compare 브랜치 이름의 첫 글자를 대문자로 쓴다 (예: `Feature/PlayerController`).
- 본문에는 이슈 번호와 구현 사항을 간략히 나열한다.
- 에러가 남아 있으면 PR을 만들지 않는다. 모든 구현이 커밋됐는지 확인한다.
- Assignee는 본인, Label은 `feature` / `enhancement` / `bug`, Milestone은 이슈의 마일스톤으로 설정한다.

**이슈**: Label은 신규 기능 `feature`, 리팩토링·개선 `enhancement`, 버그 `bug`. 마일스톤을 연결한다.

push하거나 PR이 승인되면 팀 Discord 채널에 자동으로 알림이 간다. **push는 반드시 사용자 확인 후** 한다.

## 작업 원칙

- 레거시 코드 리팩토링, 씬·프리팹 구조 변경, 매니저 간 의존성 변경은 먼저 계획을 설명하고 확인받는다.
- 요청 범위 밖의 파일은 수정하지 않는다.
