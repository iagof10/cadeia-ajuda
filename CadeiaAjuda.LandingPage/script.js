// ===== Navbar Scroll Effect =====
const navbar = document.getElementById('navbar');

window.addEventListener('scroll', () => {
    if (window.scrollY > 50) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }
});

// ===== Mobile Menu =====
const mobileMenuBtn = document.getElementById('mobileMenuBtn');
const navLinks = document.getElementById('navLinks');

mobileMenuBtn.addEventListener('click', () => {
    navLinks.classList.toggle('open');
    const icon = mobileMenuBtn.querySelector('i');
    icon.classList.toggle('fa-bars');
    icon.classList.toggle('fa-times');
});

// Close mobile menu on link click
navLinks.querySelectorAll('a').forEach(link => {
    link.addEventListener('click', () => {
        navLinks.classList.remove('open');
        const icon = mobileMenuBtn.querySelector('i');
        icon.classList.add('fa-bars');
        icon.classList.remove('fa-times');
    });
});

// ===== Browser Tabs (Platform Demo) =====
const browserTabs = document.querySelectorAll('.browser-tab');
const tabContents = document.querySelectorAll('.tab-content');

browserTabs.forEach(tab => {
    tab.addEventListener('click', () => {
        const target = tab.dataset.tab;

        browserTabs.forEach(t => t.classList.remove('active'));
        tabContents.forEach(c => c.classList.remove('active'));

        tab.classList.add('active');
        document.getElementById(`tab-${target}`).classList.add('active');
    });
});

// ===== Live Clock =====
function updateClock() {
    const clock = document.getElementById('liveClock');
    if (clock) {
        const now = new Date();
        clock.textContent = now.toLocaleTimeString('pt-BR', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    }
}

setInterval(updateClock, 1000);
updateClock();

// ===== Scroll Animations =====
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('visible');
        }
    });
}, observerOptions);

// Add fade-in to sections
document.querySelectorAll(
    '.feature-card, .step-card, .stat-card, .tech-card, .showcase-text, .showcase-visual, .cta-content'
).forEach(el => {
    el.classList.add('fade-in');
    observer.observe(el);
});

// Stagger feature cards
document.querySelectorAll('.feature-card').forEach((card, i) => {
    card.style.transitionDelay = `${i * 0.08}s`;
});

document.querySelectorAll('.stat-card').forEach((card, i) => {
    card.style.transitionDelay = `${i * 0.1}s`;
});

document.querySelectorAll('.tech-card').forEach((card, i) => {
    card.style.transitionDelay = `${i * 0.08}s`;
});

// ===== Smooth anchor scroll for all links =====
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const href = this.getAttribute('href');
        if (href === '#') return;

        const target = document.querySelector(href);
        if (target) {
            e.preventDefault();
            target.scrollIntoView({ behavior: 'smooth' });
        }
    });
});

// ===== Andon Timer Simulation =====
const timerElements = document.querySelectorAll('.andon-card-timer:not(.ok-badge)');
const liveTimerElements = document.querySelectorAll('.live-card-time:not(.ok-text)');

function parseTimer(text) {
    const match = text.match(/(\d+)m\s*(\d+)s/);
    if (match) return parseInt(match[1]) * 60 + parseInt(match[2]);
    return null;
}

function formatTimer(seconds) {
    const m = Math.floor(seconds / 60).toString().padStart(2, '0');
    const s = (seconds % 60).toString().padStart(2, '0');
    return `${m}m ${s}s`;
}

function initTimers(elements) {
    elements.forEach(el => {
        const seconds = parseTimer(el.textContent);
        if (seconds !== null) {
            el._seconds = seconds;
        }
    });
}

initTimers(timerElements);
initTimers(liveTimerElements);

setInterval(() => {
    [...timerElements, ...liveTimerElements].forEach(el => {
        if (el._seconds !== undefined) {
            el._seconds++;
            el.textContent = formatTimer(el._seconds);
        }
    });
}, 1000);
