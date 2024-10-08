// @ts-check
// `@type` JSDoc annotations allow editor autocompletion and type checking
// (when paired with `@ts-check`).
// There are various equivalent ways to declare your Docusaurus config.
// See: https://docusaurus.io/docs/api/docusaurus-config

import { themes as prismThemes } from 'prism-react-renderer';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'OpalOPC',
  tagline: 'Reveal Security Issues in your Most Critical Systems.',
  favicon: 'img/favicon.ico',

  // Set the production url of your site here
  url: 'https://opalopc.com',
  // Set the /<baseUrl>/ pathname under which your site is served
  // For GitHub pages deployment, it is often '/<projectName>/'
  baseUrl: '/',

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: 'ValtteriL', // Usually your GitHub org/user name.
  projectName: 'opalopc', // Usually your repo name.

  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  // Add scripts into <head> of HTML files.
  // https://docusaurus.io/docs/api/docusaurus-config#scripts
  scripts: [
    {
      src: 'https://umami.molemmat.fi/script.js',
      async: true,
      'data-website-id': 'e8c79fbd-9bdd-4f68-92a3-61b93464a836'
      }
  ],

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: './sidebars.js',
        },
        blog: {
          showReadingTime: true,
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      // Replace with your project's social card
      image: 'img/graphical-interface-of-vulnerability-scanner-with-logo.jpg',
      metadata: [{property: 'og:image:alt', content: 'Graphical interface of vulnerability scanner with logo on top'}],
      navbar: {
        logo: {
          alt: 'OpalOPC Logo',
          src: 'img/logo.svg',
        },
        items: [
          { to: '/', label: 'Home', position: 'right' },
          { to: '/docs', label: 'Docs', position: 'right' },
          // {to: '/blog', label: 'Blog', position: 'left'},
          // {
          //   type: 'html',
          //   position: 'right',
          //   value: '<div class="search-custom"><img src="/img/search.svg" alt="search" /><input type="text" class="search-bar" placeholder="Search OpalOPC"></div>',
          // },
        ],
      },
      algolia: {
        // The application ID provided by Algolia
        appId: 'QNZ3DE6IA2',
  
        // Public API key: it is safe to commit it
        apiKey: '66cc6c6eca56463a870e5fb1d0cecded',
  
        indexName: 'opalopc',
  
        contextualSearch: false,
        placeholder: 'Search documentation...',
      },
      // disable dark mode switch
      colorMode: {
        defaultMode: 'light',
        disableSwitch: true,
        respectPrefersColorScheme: false,
      },
      footer: {
        style: 'dark',

        links: [
          {
            items: [
              {

                html: '<img src="/img/footer.svg" alt="footer" />',
              }
            ]

          },
          {
            title: 'Docs',
            items: [
              {
                label: 'Quickstart',
                to: '/docs/get-started/quick-start',
              },
              {
                label: 'GUI Reference',
                to: '/docs/usage-gui',
              },
              {
                label: 'CLI Command Reference',
                to: '/docs/usage-cli',
              },
            ],
          },
          {
            title: 'About',
            items: [
              {
                label: 'How it works',
                to: '/docs#how-it-works',
              },
              {
                label: 'Blog',
                to: '/blog',
              },
              {
                label: 'GitHub',
                href: 'https://github.com/ValtteriL/OpalOPC',
              },
            ],
          },
          {
            title: 'Help',
            items: [
              {
                label: 'FAQ',
                to: '/docs/faq',
              },
              {
                label: 'Changelog',
                to: '/docs/changelog',
              },
              {
                label: 'Contact us',
                href: 'mailto:info@opalopc.com',
              },
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} OpalOPC · <a href="/EULA.txt">EULA</a> · <a href="/privacy-policy">Privacy</a> · <a href="/imprint">Imprint</a>`,
      },
      prism: {
        theme: prismThemes.github,
        darkTheme: prismThemes.dracula,
      },
    }),

  // plugin config
  plugins: [
    [
      '@docusaurus/plugin-client-redirects', // https://docusaurus.io/docs/api/plugins/@docusaurus/plugin-client-redirects
      {
        redirects: [
          {
            from: '/how-to-hack-opc-ua-report',
            to: '/docs/tutorials/hack-opc-ua/',
          },
          {
            from: '/how-to-hack-opc-ua',
            to: '/blog/how-to-hack-opc-ua',
          },
          {
            from: '/embed',
            to: '/',
          },
          {
            from: '/how-to-run-your-first-vulnerability-scan-with-opalopc',
            to: '/docs/tutorials/first-vulnerability-scan',
          },
          {
            from: '/linux-installer-moves-to-snap',
            to: '/blog/linux-installer-moves-to-snap',
          },
          {
            from: '/release-2000',
            to: '/blog/release-2000',
          },
          {
            from: '/buy',
            to: '/',
          },
          {
            from: '/about',
            to: '/docs/faq#who-maintains-opalopc',
          },
        ],
      },
    ],
  ],
};

export default config;
