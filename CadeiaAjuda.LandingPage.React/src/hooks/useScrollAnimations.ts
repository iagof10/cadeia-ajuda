import { useEffect, useRef } from 'react'
import gsap from 'gsap'
import { ScrollTrigger } from 'gsap/ScrollTrigger'

gsap.registerPlugin(ScrollTrigger)

export function useScrollAnimations() {
  const initialized = useRef(false)

  useEffect(() => {
    if (initialized.current) return
    initialized.current = true

    const ctx = gsap.context(() => {
      // === Hero parallax on scroll ===
      gsap.to('.hero-content', {
        y: 120,
        opacity: 0.2,
        ease: 'none',
        scrollTrigger: { trigger: '.hero', start: 'top top', end: 'bottom top', scrub: true },
      })

      gsap.to('.hero-bg-effects', {
        y: 60,
        scale: 1.05,
        ease: 'none',
        scrollTrigger: { trigger: '.hero', start: 'top top', end: 'bottom top', scrub: true },
      })

      // === Stats cards ===
      gsap.from('.stat-card', {
        scrollTrigger: { trigger: '.stats-section', start: 'top 85%', once: true },
        y: 50,
        opacity: 0,
        scale: 0.85,
        duration: 0.7,
        stagger: 0.1,
        ease: 'back.out(1.7)',
      })

      // === Platform demo ===
      const platformTl = gsap.timeline({
        scrollTrigger: { trigger: '.platform-section', start: 'top 70%', once: true },
      })
      platformTl
        .from('.browser-frame', {
          y: 60,
          opacity: 0,
          scale: 0.94,
          duration: 1,
          ease: 'power3.out',
        })
        .from('.browser-tab', {
          y: -15,
          opacity: 0,
          duration: 0.35,
          stagger: 0.06,
          ease: 'power2.out',
        }, '-=0.5')

      // === Feature cards stagger ===
      gsap.from('.feature-card', {
        scrollTrigger: { trigger: '.features-section', start: 'top 75%', once: true },
        y: 60,
        opacity: 0,
        duration: 0.7,
        stagger: { each: 0.06, from: 'start' },
        ease: 'power3.out',
      })

      // === Andon showcase cinematic ===
      const showcaseTl = gsap.timeline({
        scrollTrigger: { trigger: '.andon-showcase', start: 'top 70%', once: true },
      })
      showcaseTl
        .from('.showcase-text h2', { x: -60, opacity: 0, duration: 0.8, ease: 'power3.out' })
        .from('.showcase-text p', { x: -40, opacity: 0, duration: 0.6, ease: 'power3.out' }, '-=0.4')
        .from('.showcase-features li', { x: -30, opacity: 0, duration: 0.4, stagger: 0.08, ease: 'power3.out' }, '-=0.3')
        .from('.showcase-text .btn', { y: 15, opacity: 0, duration: 0.4, ease: 'back.out(1.7)' }, '-=0.2')
        .from('.andon-live-panel', { x: 80, opacity: 0, rotateY: -12, duration: 1, ease: 'power3.out' }, '-=1')

      gsap.from('.andon-gif', {
        scrollTrigger: { trigger: '.andon-live-panel', start: 'top 80%', once: true },
        scale: 0.94,
        opacity: 0,
        duration: 0.8,
        ease: 'power3.out',
      })

      // === Timeline items ===
      gsap.from('.timeline-item', {
        scrollTrigger: { trigger: '.how-section', start: 'top 70%', once: true },
        x: -40,
        opacity: 0,
        duration: 0.7,
        stagger: 0.2,
        ease: 'power3.out',
      })

      gsap.from('.timeline-line', {
        scrollTrigger: { trigger: '.how-section', start: 'top 70%', once: true },
        scaleY: 0,
        transformOrigin: 'top center',
        duration: 1.2,
        ease: 'power3.out',
      })

      // === Tech cards ===
      gsap.from('.tech-card', {
        scrollTrigger: { trigger: '.tech-section', start: 'top 80%', once: true },
        y: 30,
        opacity: 0,
        scale: 0.92,
        duration: 0.5,
        stagger: { each: 0.06, from: 'center' },
        ease: 'back.out(1.5)',
      })

      // === CTA ===
      gsap.from('.cta-card', {
        scrollTrigger: { trigger: '.cta-section', start: 'top 80%', once: true },
        y: 50,
        opacity: 0,
        scale: 0.9,
        duration: 1,
        ease: 'power3.out',
      })

      // === Section titles ===
      gsap.utils.toArray<HTMLElement>('.section-title').forEach((el) => {
        gsap.from(el, {
          scrollTrigger: { trigger: el, start: 'top 85%', once: true },
          y: 35,
          opacity: 0,
          duration: 0.8,
          ease: 'power3.out',
        })
      })

      gsap.utils.toArray<HTMLElement>('.section-badge').forEach((el) => {
        gsap.from(el, {
          scrollTrigger: { trigger: el, start: 'top 88%', once: true },
          y: 15,
          opacity: 0,
          letterSpacing: '6px',
          duration: 0.7,
          ease: 'power3.out',
        })
      })

      gsap.utils.toArray<HTMLElement>('.section-subtitle').forEach((el) => {
        gsap.from(el, {
          scrollTrigger: { trigger: el, start: 'top 85%', once: true },
          y: 20,
          opacity: 0,
          duration: 0.7,
          delay: 0.1,
          ease: 'power3.out',
        })
      })

      // === Footer ===
      gsap.from('.footer-content', {
        scrollTrigger: { trigger: '.footer', start: 'top 90%', once: true },
        y: 25,
        opacity: 0,
        duration: 0.6,
        ease: 'power3.out',
      })

      // === Magnetic buttons ===
      document.querySelectorAll<HTMLElement>('.btn-magnetic').forEach((btn) => {
        const handleMove = (e: MouseEvent) => {
          const rect = btn.getBoundingClientRect()
          const x = e.clientX - rect.left - rect.width / 2
          const y = e.clientY - rect.top - rect.height / 2
          gsap.to(btn, { x: x * 0.25, y: y * 0.25, duration: 0.3, ease: 'power2.out' })
        }
        const handleLeave = () => {
          gsap.to(btn, { x: 0, y: 0, duration: 0.5, ease: 'elastic.out(1, 0.3)' })
        }
        btn.addEventListener('mousemove', handleMove)
        btn.addEventListener('mouseleave', handleLeave)
      })
    })

    return () => ctx.revert()
  }, [])
}
