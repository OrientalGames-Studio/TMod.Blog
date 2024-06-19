using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Web.Services.Abstraction;

namespace TMod.Blog.Web.Services.Client
{
    internal sealed partial class IconPathProviderService : IIconPathProviderService
    {
        public string BrandIcon => """<path d="M384 64c0 35.4 28.6 64 64 64 247.4 0 448 200.6 448 448 0 35.4 28.6 64 64 64s64-28.6 64-64C1024 257.8 766.2 0 448 0c-35.4 0-64 28.6-64 64z m0 192c0 35.4 28.6 64 64 64 141.4 0 256 114.6 256 256 0 35.4 28.6 64 64 64s64-28.6 64-64c0-212-172-384-384-384-35.4 0-64 28.6-64 64z m-192 32c0-53-43-96-96-96S0 235 0 288v448c0 159 129 288 288 288s288-129 288-288-129-288-288-288h-32v192h32c53 0 96 43 96 96s-43 96-96 96-96-43-96-96V288z" p-id="4312"></path>""";

        public string LightModeIcon => """
            <?xml version="1.0" encoding="utf-8"?>
            <!-- License: Apache. Made by Iconscout: https://github.com/Iconscout/unicons -->
            <svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path d="M5.64,17l-.71.71a1,1,0,0,0,0,1.41,1,1,0,0,0,1.41,0l.71-.71A1,1,0,0,0,5.64,17ZM5,12a1,1,0,0,0-1-1H3a1,1,0,0,0,0,2H4A1,1,0,0,0,5,12Zm7-7a1,1,0,0,0,1-1V3a1,1,0,0,0-2,0V4A1,1,0,0,0,12,5ZM5.64,7.05a1,1,0,0,0,.7.29,1,1,0,0,0,.71-.29,1,1,0,0,0,0-1.41l-.71-.71A1,1,0,0,0,4.93,6.34Zm12,.29a1,1,0,0,0,.7-.29l.71-.71a1,1,0,1,0-1.41-1.41L17,5.64a1,1,0,0,0,0,1.41A1,1,0,0,0,17.66,7.34ZM21,11H20a1,1,0,0,0,0,2h1a1,1,0,0,0,0-2Zm-9,8a1,1,0,0,0-1,1v1a1,1,0,0,0,2,0V20A1,1,0,0,0,12,19ZM18.36,17A1,1,0,0,0,17,18.36l.71.71a1,1,0,0,0,1.41,0,1,1,0,0,0,0-1.41ZM12,6.5A5.5,5.5,0,1,0,17.5,12,5.51,5.51,0,0,0,12,6.5Zm0,9A3.5,3.5,0,1,1,15.5,12,3.5,3.5,0,0,1,12,15.5Z"/></svg>
            """;

        public string DarkModeIcon => """
            <?xml version="1.0" encoding="UTF-8"?>
            
            <!-- License: Apache. Made by Richard9394: https://github.com/Richard9394/MingCute -->
            <svg viewBox="0 0 24 24" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
                <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                    <g id="Weather" transform="translate(-96.000000, 0.000000)">
                        <g id="moon_line" transform="translate(96.000000, 0.000000)">
                            <path d="M24,0 L24,24 L0,24 L0,0 L24,0 Z M12.5934901,23.257841 L12.5819402,23.2595131 L12.5108777,23.2950439 L12.4918791,23.2987469 L12.4918791,23.2987469 L12.4767152,23.2950439 L12.4056548,23.2595131 C12.3958229,23.2563662 12.3870493,23.2590235 12.3821421,23.2649074 L12.3780323,23.275831 L12.360941,23.7031097 L12.3658947,23.7234994 L12.3769048,23.7357139 L12.4804777,23.8096931 L12.4953491,23.8136134 L12.4953491,23.8136134 L12.5071152,23.8096931 L12.6106902,23.7357139 L12.6232938,23.7196733 L12.6232938,23.7196733 L12.6266527,23.7031097 L12.609561,23.275831 C12.6075724,23.2657013 12.6010112,23.2592993 12.5934901,23.257841 L12.5934901,23.257841 Z M12.8583906,23.1452862 L12.8445485,23.1473072 L12.6598443,23.2396597 L12.6498822,23.2499052 L12.6498822,23.2499052 L12.6471943,23.2611114 L12.6650943,23.6906389 L12.6699349,23.7034178 L12.6699349,23.7034178 L12.678386,23.7104931 L12.8793402,23.8032389 C12.8914285,23.8068999 12.9022333,23.8029875 12.9078286,23.7952264 L12.9118235,23.7811639 L12.8776777,23.1665331 C12.8752882,23.1545897 12.8674102,23.1470016 12.8583906,23.1452862 L12.8583906,23.1452862 Z M12.1430473,23.1473072 C12.1332178,23.1423925 12.1221763,23.1452606 12.1156365,23.1525954 L12.1099173,23.1665331 L12.0757714,23.7811639 C12.0751323,23.7926639 12.0828099,23.8018602 12.0926481,23.8045676 L12.108256,23.8032389 L12.3092106,23.7104931 L12.3186497,23.7024347 L12.3186497,23.7024347 L12.3225043,23.6906389 L12.340401,23.2611114 L12.337245,23.2485176 L12.337245,23.2485176 L12.3277531,23.2396597 L12.1430473,23.1473072 Z" id="MingCute" fill-rule="nonzero">
            
            </path>
                            <path d="M13.574,3.13729 C12.7837,2.99779 12.1416,3.79855 12.4769,4.54557 C12.8127,5.29352 13,6.12352 13,6.99996 C13,10.3137 10.3137,13 7,13 C6.12356,13 5.29356,12.8127 4.54561,12.4769 C3.79866,12.1416 2.99783,12.7836 3.13734,13.5739 C3.8823,17.7941 7.56575,21 12,21 C16.9706,21 21,16.9705 21,12 C21,7.5657 17.7942,3.88225 13.574,3.13729 Z M14.8809,5.61808 C17.3098,6.71634 19,9.16146 19,12 C19,15.8659 15.866,19 12,19 C9.1615,19 6.71639,17.3098 5.61813,14.8809 C6.06734,14.9592 6.52913,15 7,15 C11.4183,15 15,11.4182 15,6.99996 C15,6.52909 14.9592,6.0673 14.8809,5.61808 Z" id="形状" fill="#09244B">
            
            </path>
                        </g>
                    </g>
                </g>
            </svg>
            """;

        public string EditIcon => """
             <?xml version="1.0" encoding="utf-8"?>
            <!-- License: CC Attribution. Made by Amir Baqian: https://dribbble.com/amirbaqian -->
            <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M9.34311 4.69205C10.2169 4.48709 11.1084 4.38462 12 4.38462C12.3823 4.38462 12.6923 4.07466 12.6923 3.69231C12.6923 3.30996 12.3823 3 12 3C11.0023 3 10.0047 3.11468 9.02691 3.34403C6.20715 4.00545 4.00545 6.20715 3.34403 9.02691C2.88532 10.9824 2.88532 13.0176 3.34403 14.9731C4.00546 17.7929 6.20715 19.9945 9.02691 20.656C10.9824 21.1147 13.0176 21.1147 14.9731 20.656C17.7928 19.9945 19.9945 17.7929 20.656 14.9731C20.8853 13.9953 21 12.9976 21 12C21 11.6176 20.69 11.3077 20.3077 11.3077C19.9253 11.3077 19.6154 11.6176 19.6154 12C19.6154 12.8915 19.5129 13.7831 19.3079 14.6569C18.7666 16.9647 16.9647 18.7666 14.6569 19.3079C12.9093 19.7179 11.0907 19.7179 9.34312 19.3079C7.03533 18.7666 5.23339 16.9647 4.69205 14.6569C4.28214 12.9093 4.28214 11.0907 4.69205 9.34311C5.23339 7.03533 7.03533 5.23339 9.34311 4.69205Z" fill="#363853"/>
            <path fill-rule="evenodd" clip-rule="evenodd" d="M16.2626 3.69309C16.781 3.24745 17.4438 3 18.1321 3C19.716 3 21 4.28398 21 5.86785C21 6.55619 20.7526 7.21898 20.3069 7.73736C20.2601 7.7918 20.2111 7.84464 20.16 7.89573L16.8458 11.21C15.3162 12.7395 13.3997 13.8246 11.3012 14.3493L10.6606 14.5094C9.95401 14.6861 9.31394 14.046 9.4906 13.3394L9.65074 12.6988C10.1754 10.6003 11.2605 8.68377 12.79 7.15421L16.1043 3.83997C16.1554 3.78888 16.2082 3.7399 16.2626 3.69309ZM18.253 7.84465C17.7729 7.65101 17.3389 7.34317 16.9979 7.00214C16.6568 6.6611 16.349 6.22708 16.1554 5.74703L13.7691 8.13328C12.4263 9.47608 11.471 11.1562 11.0037 12.9963C12.8438 12.529 14.5239 11.5737 15.8667 10.2309L18.253 7.84465Z" fill="#363853"/>
            </svg>
            """;

        public string ListIcon => """
             <?xml version="1.0" encoding="UTF-8" standalone="no"?>
            
            
            
            <!-- License: BSD. Made by Nuiverse Design: https://gitlab.com/nuinalp/open-source/nuiverse/icons -->
            <svg 
               xmlns:dc="http://purl.org/dc/elements/1.1/"
               xmlns:cc="http://creativecommons.org/ns#"
               xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
               xmlns:svg="http://www.w3.org/2000/svg"
               xmlns="http://www.w3.org/2000/svg"
               xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
               xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
               viewBox="0 0 30 30"
               version="1.1"
               id="svg822"
               inkscape:version="0.92.4 (f8dce91, 2019-08-02)"
               sodipodi:docname="list.svg">
              <defs
                 id="defs816" />
              <sodipodi:namedview
                 id="base"
                 borderopacity="1.0"
                 inkscape:pageopacity="0.0"
                 inkscape:pageshadow="2"
                 inkscape:zoom="17.833333"
                 inkscape:cx="15"
                 inkscape:cy="15"
                 inkscape:document-units="px"
                 inkscape:current-layer="layer1"
                 showgrid="true"
                 units="px"
                 inkscape:window-width="1366"
                 inkscape:window-height="713"
                 inkscape:window-x="0"
                 inkscape:window-y="0"
                 inkscape:window-maximized="1"
                 showguides="false">
                <inkscape:grid
                   type="xygrid"
                   id="grid816" />
              </sodipodi:namedview>
              <metadata
                 id="metadata819">
                <rdf:RDF>
                  <cc:Work
                     rdf:about="">
                    <dc:format>image/svg+xml</dc:format>
                    <dc:type
                       rdf:resource="http://purl.org/dc/dcmitype/StillImage" />
                    <dc:title>
            
            </dc:title>
                  </cc:Work>
                </rdf:RDF>
              </metadata>
              <g
                 inkscape:label="Layer 1"
                 inkscape:groupmode="layer"
                 id="layer1"
                 transform="translate(0,-289.0625)">
                <rect
                   
                   id="rect887"
                   width="17"
                   height="2"
                   x="3"
                   y="296.0625"
                   rx="1" />
                <rect
                   y="303.0625"
                   x="3"
                   height="2"
                   width="20"
                   id="rect889"
                   
                   rx="1" />
                <rect
                   
                   id="rect891"
                   width="12"
                   height="2"
                   x="3"
                   y="310.0625"
                   rx="1" />
              </g>
            </svg>
            """;

        public string PreviewIcon => """
             <?xml version="1.0" encoding="UTF-8" standalone="no"?>
            
            
            
            <!-- License: PD. Made by jimlamb: https://github.com/jimlamb/bowtie -->
            <svg 
               xmlns:dc="http://purl.org/dc/elements/1.1/"
               xmlns:cc="http://creativecommons.org/ns#"
               xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
               xmlns:svg="http://www.w3.org/2000/svg"
               xmlns="http://www.w3.org/2000/svg"
               xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
               xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
               viewBox="0 0 448 448"
               id="svg2"
               version="1.1"
               inkscape:version="0.91 r13725"
               sodipodi:docname="file-preview.svg">
              <title
                 id="title3352">file-preview</title>
              <defs
                 id="defs4" />
              <sodipodi:namedview
                 id="base"
                 pagecolor="#ffffff"
                 bordercolor="#666666"
                 borderopacity="1.0"
                 inkscape:pageopacity="0.0"
                 inkscape:pageshadow="2"
                 inkscape:zoom="0.98994949"
                 inkscape:cx="337.14194"
                 inkscape:cy="277.94583"
                 inkscape:document-units="px"
                 inkscape:current-layer="layer1"
                 showgrid="true"
                 fit-margin-top="448"
                 fit-margin-right="384"
                 fit-margin-left="0"
                 fit-margin-bottom="0"
                 units="px"
                 inkscape:window-width="1196"
                 inkscape:window-height="852"
                 inkscape:window-x="132"
                 inkscape:window-y="423"
                 inkscape:window-maximized="0"
                 inkscape:snap-bbox="true"
                 inkscape:bbox-nodes="true">
                <inkscape:grid
                   type="xygrid"
                   id="grid3347"
                   spacingx="16"
                   spacingy="16"
                   empspacing="2"
                   originx="0"
                   originy="-1.7498462e-005" />
              </sodipodi:namedview>
              <metadata
                 id="metadata7">
                <rdf:RDF>
                  <cc:Work
                     rdf:about="">
                    <dc:format>image/svg+xml</dc:format>
                    <dc:type
                       rdf:resource="http://purl.org/dc/dcmitype/StillImage" />
                    <dc:title>file-preview</dc:title>
                  </cc:Work>
                </rdf:RDF>
              </metadata>
              <g
                 inkscape:label="Layer 1"
                 inkscape:groupmode="layer"
                 id="layer1"
                 transform="translate(0,-604.36224)">
                <path
                   style="fill-opacity:1;stroke:none;stroke-width:36;stroke-linecap:round;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1"
                   d="M 96 0 L 96 32 L 96 160 L 128 160 L 128 32 L 320 32 L 320 128 L 416 128 L 416 416 L 160 416 L 144 416 L 128 416 L 96 416 L 96 448 L 128 448 L 448 448 L 448 416 L 448 111.01953 L 336.98047 0 L 128 0 L 96 0 z "
                   transform="translate(0,604.36224)"
                   id="rect3338" />
                <path
                   style="fill-opacity:1;stroke:none;stroke-width:36;stroke-linecap:round;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1"
                   d="M 160 192 A 96 95.999992 0 0 0 64 288 A 96 95.999992 0 0 0 78.117188 337.88281 L 0 416 L 32 448 L 110.09375 369.90625 A 96 95.999992 0 0 0 160 384 A 96 95.999992 0 0 0 256 288 A 96 95.999992 0 0 0 160 192 z M 160 224 A 64 63.999996 0 0 1 224 288 A 64 63.999996 0 0 1 160 352 A 64 63.999996 0 0 1 96 288 A 64 63.999996 0 0 1 160 224 z "
                   transform="translate(0,604.36224)"
                   id="path3336" />
              </g>
            </svg>
            """;

        public string ConfigurationIcon => """
             <?xml version="1.0" encoding="utf-8"?>
            <!-- License: MIT. Made by etn-ccis: https://github.com/etn-ccis/blui-icons -->
            <svg viewBox="0 0 24 24"  xmlns="http://www.w3.org/2000/svg">
            <path d="M7 3L5 3L5 9H7L7 3ZM19 3L17 3V13H19V3ZM3 13H5L5 21H7L7 13H9V11H3L3 13ZM15 7H13V3L11 3V7L9 7L9 9L15 9V7ZM11 21H13V11H11L11 21ZM15 15V17L17 17V21H19V17H21V15L15 15Z" />
            </svg>
            """;

        public string ReportIcon => """
             <?xml version="1.0" encoding="utf-8"?>
            <!-- License: MIT. Made by Shopify: https://github.com/Shopify/polaris -->
            <svg viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path d="M18 5a1 1 0 00-.293-.707l-4-4A1.002 1.002 0 0013 0H3.5A1.5 1.5 0 002 1.5V6a1 1 0 102 0V2h8.586L16 5.414V18H4v-1a1 1 0 10-2 0v1.5A1.5 1.5 0 003.5 20h13a1.5 1.5 0 001.5-1.5V5z"/><path d="M9 6a1 1 0 000 2h3v3a1 1 0 102 0V7a1 1 0 00-1-1H9z"/><path d="M7 13l-.768.64a1 1 0 001.475.067L7 13zm-2.5-3l.768-.64a1 1 0 00-1.52-.018L4.5 10zM.247 13.341a1 1 0 001.506 1.318L.247 13.34zm11.046-6.048l-5 5 1.414 1.414 5-5-1.414-1.414zM7.768 12.36l-2.5-3-1.536 1.28 2.5 3 1.536-1.28zm-4.02-3.018l-3.5 4 1.505 1.316 3.5-4-1.506-1.316z"/></svg>
            """;
    }
}
