// @ts-check
// `@type` JSDoc annotations allow editor autocompletion and type checking
// (when paired with `@ts-check`).
// There are various equivalent ways to declare your Docusaurus config.
// See: https://docusaurus.io/docs/api/docusaurus-config

import { themes as prismThemes } from 'prism-react-renderer';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'OpalOPC',
  tagline: 'Dinosaurs are cool',
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

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: './sidebars.js',
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl:
            'https://github.com/facebook/docusaurus/tree/main/packages/create-docusaurus/templates/shared/',
        },
        blog: {
          showReadingTime: true,
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl:
            'https://github.com/facebook/docusaurus/tree/main/packages/create-docusaurus/templates/shared/',
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
      // image: 'img/docusaurus-social-card.jpg',
      navbar: {
        logo: {
          alt: 'OpalOPC Logo',
          src: 'img/logo.svg',
        },
        items: [
          { to: '/', label: 'Home', position: 'right' },
          { to: '/docs', label: 'Docs', position: 'right' },
          // {to: '/blog', label: 'Blog', position: 'left'},
          {
            type: 'html',
            position: 'right',
            value: '<div class="search-custom"><img src="/img/search.svg" alt="search" /><input type="text" class="search-bar" placeholder="Search OpalOPC"></div>',
          },
        ],
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
                label: 'YouTube',
                href: 'https://www.youtube.com/playlist?list=PL0CpXbWypOUn3QcYLOMghfV2bh5nGWut6',
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
        copyright: `Copyright © ${new Date().getFullYear()} OpalOPC · <a href="/EULA.txt">EULA</a> · <a href="/privacy-policy">Privacy</a>`,
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
          // How to pentest OPC UA presentation link to mailchimp landing page
          {
            from: '/how-to-hack-opc-ua-report',
            to: 'https://mailchi.mp/opalopc/how-to-hack-opc-ua',
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
