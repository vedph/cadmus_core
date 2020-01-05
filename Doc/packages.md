# Backend Packages

The backend is fully implemented in C# and .NET Core. Shared dependencies are packaged into NuGet packages.

We can describe Cadmus backend packages into these logical groups:

- `Fusi`: general-purpose components, preexisting the Cadmus project and created and used in several other systems. These are generic dependencies, like any other 3rd party package dependency.
- `Cadmus`: Cadmus core components.
- `Cadmus.Parts`: Cadmus parts and fragments "plugins".
- `Cadmus.Seed`: Cadmus core components for seeding mock data.
- `Cadmus.Seed.Parts`: Cadmus parts and fragments seeders "plugins".
- `Messaging.Api`: messaging-related tools for API layers. This is used to compose and send email messages, through various SMTP services. API layers use this to message users (e.g. confirm registration, reset password, etc.).
- `CadmusTool`: the CLI tool for Cadmus.
- `CadmusApi`: the Cadmus API layer.

```plantuml
@startuml
    skinparam backgroundColor #EEEBDC
    skinparam handwritten true

package Fusi {
    class Fusi_Tools
    class Fusi_Tools_Config
    class Fusi_Text
}

package Cadmus {
  Fusi_Tools <|-- Cadmus_Core
  Fusi_Text <|-- Cadmus_Core

  Cadmus_Core <|-- Cadmus_Mongo
  Cadmus_Core <|-- Cadmus_Parts
}

package Cadmus.Parts {
  Fusi_Antiquity <|-- Cadmus_Parts

  Cadmus_Core <|-- Cadmus_Philology_Parts
  Fusi_Antiquity <|-- Cadmus_Philology_Parts

  Cadmus_Core <|-- Cadmus_Archive_Parts

  Cadmus_Core <|-- Cadmus_Lexicon_Parts
  Fusi_Antiquity <|-- Cadmus_Lexicon_Parts
}

package Cadmus.Seed {
  Cadmus_Core <|-- Cadmus_Seed
  Fusi_Tools_Config <|-- Cadmus_Seed
  Fusi_Tools <|-- Fusi_Tools_Config
}

package Cadmus.Seed.Parts {
  Cadmus_Seed <|-- Cadmus_Seed_Parts
  Cadmus_Parts <|-- Cadmus_Seed_Parts

  Cadmus_Seed <|-- Cadmus_Seed_Philology_Parts
  Cadmus_Philology_Parts <|-- Cadmus_Seed_Philology_Parts
}

package CadmusTool {
  Fusi_Tools <|-- CadmusTool
  Fusi_Tools_Config <|-- CadmusTool
  Fusi_Text <|-- CadmusTool
  Cadmus_Core <|-- CadmusTool
  Cadmus_Mongo <|-- CadmusTool
  Cadmus_Parts <|-- CadmusTool
  Cadmus_Philology_Parts <|-- CadmusTool
  Cadmus_Lexicon_Parts <|-- CadmusTool
  Cadmus_Seed <|-- CadmusTool
}

package MessagingApi {
  class MessagingApi
  MessagingApi <|-- MessagingApi_SendGrid
  MessagingApi <|-- MessagingApi_MailJet
}

package CadmusApi {
  Fusi_Antiquity <|-- CadmusApi
  MessagingApi <|-- CadmusApi
  MessagingApi_SendGrid <|-- CadmusApi
  Cadmus_Core <|-- CadmusApi
  Cadmus_Mongo <|-- CadmusApi
  Cadmus_Parts <|-- CadmusApi
  Cadmus_Philology_Parts <|-- CadmusApi
  Cadmus_Lexicon_Parts <|-- CadmusApi
  Cadmus_Seed <|-- CadmusApi
  Cadmus_Seed_Parts <|-- CadmusApi
  Cadmus_Seed_Philology_Parts <|-- CadmusApi
}
@enduml
```

## NuGet Dependencies

According to the above overview, these are the NuGet dependencies for Cadmus:

- `DiffMatchPatch`
- `Fusi.Tools`
- `Fusi.Tools.Config`
- `Fusi.Text`
- `Fusi.Antiquity`
- `MessagingApi`
- `MessagingApi.SendGrid`

Cadmus-specific:

- `Cadmus.Core`
- `Cadmus.Mongo`
- `Cadmus.Seed`
- `Cadmus.Parts`
- `Cadmus.Seed.Parts`
- `Cadmus.Philology.Parts`
- `Cadmus.Seed.Philology.Parts`
- `Cadmus.Archive.Parts`
- `Cadmus.Lexicon.Parts`
