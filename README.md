# JumpingSquare
게임 서버-클라이언트 구조를 공부/구현하기 위해 개발한 멀티플레이 점프맵 게임 프로젝트입니다.

[Game Server Project]
[GitHub: JumpingSquare Server](https://github.com/DecisionDisorder/JumpingSquare_MainServer "[GitHub: JumpingSquare Server]")  

[Developer]
- 김현종 (guswhd990507@naver.com)

## 프로젝트 소개
### [게임설명]
- 용암 등으로 떨어지면 플레이어가 사망하는 대형 컨테이너 박스에서 안전하게 점프하며 위험한 공간을 탈출하는 게임입니다.
- 키보드 w, a, s, d로 캐릭터를 움직일 수 있으며, 마우스로 1인칭 화면을 회전할 수 있습니다.
- 키보드 shift키를 눌러서 달리기를 할 수 있습니다.

### [기획의도]
마인크래프트의 점프 맵(탈출 맵)을 만들었던 과거의 추억과 현재의 기술적 학습의 방향성을 하나로 모아서 마인크래프트 점프 맵 스타일의 멀티플레이 게임을 만들자는 의도를 가지고 개발하였습니다.

### [데모 영상]
- https://

## 기술 관련 소개
###  [개발환경 및 기술 스택]
- Unity 2022.2.5f1 + C#
- Visual Studio 2022
- C# .Net (Socket)
- VCS: GitHub

### [개발 이슈]
**서버에 데이터를 보내는 간격: 0.1초**
- 로컬 환경에서는 매 프레임마다 서버와 클라이언트가 데이터를 주고받을 수 있지만, 네트워크 환경에서는 물리적인 딜레이를 고려해야 합니다. 서버를 개발하며 참고한 도서(게임 서버 프로그래밍 교과서)에서는 통상적으로 100ms(0.1초) 간격으로 전송한다는 정보를 보고 업데이트를 0.1초 간격으로 설정하였습니다.
- 캐릭터의 위치를 0.1초마다 업데이트하면 끊기는 것이 눈에 보이기 때문에 interpolation이 필요한데, 그중에서 Unity 엔진에서 제공하는 3차원 공간에서의 선형 보간 함수(Vector3.Lerp)를 사용하여 사용자 눈에 부드럽게 이동하는 것으로 보이도록 처리하였습니다.
(관련 코드: [CommunicateManager.cs](https://github.com/DecisionDisorder/JumpingSquare/blob/master/Assets/CommunicateManager.cs "CommunicateManager.cs"))

**플레이어의 응답성**
- 사용자가 직접 조작하는 캐릭터는 키보드를 누르는 즉시, 마우스를 돌리는 즉시 응답이 있어야 원활한 게임 플레이 경험이 가능합니다. 그런데 서버에 입력 값을 전달하고 처리 결과를 클라이언트에 보이도록 하면, 사용자는 캐릭터 조작감에 있어서 불편함을 느낄 수밖에 없기 때문에 클라이언트에서 바로 처리하도록 구현하였습니다.
- 다만, 클라이언트에서 이동하는 것을 서버에서 그대로 모두 받아들인다면, 클라이언트를 해킹했을 때 캐릭터가 비정상적으로 움직이는 모든 것이 반영되어 게임의 공정성에 문제가 발생하기 때문에 서버에서 이동속도를 계산하여 유효 범위 내에 들어오는지 확인하는 과정이 추가적으로 필요한 것으로 사료됩니다.
(관련 코드: [Player.cs](https://github.com/DecisionDisorder/JumpingSquare/blob/master/Assets/Player.cs "Player.cs"))
