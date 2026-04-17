export default function Footer() {
  return (
    <footer className="footer">
      <div className="container">
        <div className="footer-content">
          <div className="footer-brand">
            <a href="#" className="nav-logo" aria-label="AssistChain">
              <img src="/image/logo/AssistChain-logo.png" alt="AssistChain" className="logo-img logo-footer" />
            </a>
          </div>
          <div className="footer-links">
            <a href="#funcionalidades">Funcionalidades</a>
            <a href="#como-funciona">Como Funciona</a>
            <a href="#tecnologias">Tecnologias</a>
            <a href="#contato">Contato</a>
          </div>
        </div>
        <div className="footer-bottom">
          <p>&copy; 2026 AssistChain. Todos os direitos reservados.</p>
          <div className="footer-legal">
            <a href="#">Termos de Serviço</a>
            <a href="#">Política de Privacidade</a>
          </div>
        </div>
      </div>
    </footer>
  )
}
