# Arsalan Pathan — Personal Portfolio

A modern and responsive personal portfolio website created to present my professional profile, technical skills, services, education, internship experience and contact information.

## Live Demo

[View the deployed portfolio](https://arsalan-pathan-portfolio.vercel.app/)

## Features

- Responsive navigation with a mobile menu
- Professional hero section and profile image
- About, skills and technologies sections
- Reusable service cards
- Internship experience and education timeline
- Data-driven project system with an empty state
- Certifications and resume placeholders
- Validated contact form
- GitHub, LinkedIn and email integration
- Basic SEO and Open Graph metadata
- Keyboard-friendly navigation and accessibility improvements
- Responsive layouts for desktop, tablet and mobile

## Technologies Used

- React
- JavaScript (ES6+)
- JSX
- CSS3
- Vite
- Lucide React
- Git and GitHub

## Getting Started

### Prerequisites

Install a current LTS version of [Node.js](https://nodejs.org/).

### Installation

```bash
git clone <repository-url>
cd project-01-personal-portfolio-professional-profile
npm install
```

### Run the Development Server

```bash
npm run dev
```

Open the local URL displayed in the terminal, normally `http://localhost:5173`.

### Create a Production Build

```bash
npm run build
```

### Preview the Production Build

```bash
npm run preview
```

## Project Structure

```text
├── src/
│   ├── assets/          # Profile and website images
│   ├── data/            # Data-driven project content
│   ├── App.jsx          # Main React application
│   ├── main.jsx         # React entry point
│   └── styles.css       # Responsive website styling
├── index.html           # SEO metadata and page entry
├── package.json         # Dependencies and scripts
└── vite.config.js       # Vite configuration
```

## Adding a Project

Add a project object to `src/data/projects.js`:

```js
{
  title: "Project Name",
  description: "A short project description.",
  image: "/project-images/project-name.webp",
  technologies: ["React", "ASP.NET", "SQL Server"],
  features: ["Responsive design", "CRUD operations"],
  github: "https://github.com/username/repository",
  demo: "https://example.com"
}
```

## Future Improvements

- Add completed projects and live demos
- Add professional certifications
- Integrate an updated resume PDF
- Connect the contact form to an email service or backend API
- Add a dynamic blog and optional administration panel
- Add a canonical URL after deployment

## Author

**Arsalan Pathan** — Full-Stack Developer

- [GitHub](https://github.com/arslan23111)
- [LinkedIn](https://www.linkedin.com/in/arsalan-pathan-55b78a299/)
- Email: lalaarslanpathan14@gmail.com
- Location: Hyderabad, Sindh, Pakistan

## License

This project is intended for personal portfolio and educational use.
