import { Metadata } from 'next';

import { LinkResults } from '@/components/LinkResults';
import { TagCloud } from '@/components/tags/TagCloud';
import { WithRightSidebar } from '@/components/layouts/WithRightSidebar';
import { useGetTagsAsArray } from '@/hooks/useGetTagsAsArray';

type Props = {
   searchParams?: { [key: string]: string | string[] | undefined };
};

export async function generateMetadata({
   searchParams,
}: Props): Promise<Metadata> {
   const params = (await searchParams) || {};

   const page = params?.page ? Number(params.page) : 1;

   const tagsArr = useGetTagsAsArray(params.tags, false);
   const tagsSortedArr = useGetTagsAsArray(tagsArr, true);
   const tagsSortedStr = tagsSortedArr.join(', ').trim();

   let title = 'Links';

   if (tagsSortedArr.length > 0) {
      title += ` tagged with "${tagsSortedStr}"`;
   }

   if (page && page > 1) {
      title += ` - Page ${page}`;
   }

   // For the canonical, string out the ', ' and replace ' ' with '+' for URL encoding
   const canonicalTags =
      tagsSortedStr.length > 0
         ? `?tags=${tagsSortedStr.replaceAll(', ', ',').replace(/ /g, '+')}`
         : '';

   return {
      title: (title += ' | Delisc.io'),
      description: title,
      alternates: {
         canonical: `/links${canonicalTags.length > 0 ? `?tags=${canonicalTags}` : ''}`,
      },
   };
}

export default async function LinksPage({
   searchParams,
}: {
   searchParams?: { [key: string]: string | string[] | undefined };
}) {
   const params = (await searchParams) || {};

   const tags = params?.tags
      ? Array.isArray(params.tags)
         ? params.tags
         : params.tags.split(',')
      : [];

   const title = `${tags.length > 0 ? `Links for "${tags.join(' + ')}"` : 'Links'}`;

   return (
      <>
         <h1>{title}</h1>
         <WithRightSidebar
            main={<LinkResults searchParams={params} />}
            sidebar={<TagCloud maxTags={150} currentTags={tags} />}
         />
      </>
   );
}
