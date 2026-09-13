# URDev Game Framework

Script-only Unity game framework package.

## Package Identity

- Package name: `com.urdev.game-framework`
- Root namespace: `UR.GameFramework`
- Runtime assembly: `UR.GameFramework`
- Minimum Unity version: `2022.3`

## Structure

```text
Packages/com.urdev.game-framework
├── Runtime
│   ├── URDev.GameFramework.asmdef
│   └── Framework.cs
├── Docs
│   └── PackageAuthoringGuide.md
├── CHANGELOG.md
├── README.md
└── package.json
```

Add runtime framework code under `Runtime` and keep it inside the `URDev.GameFramework` namespace unless a sub-namespace is needed.
